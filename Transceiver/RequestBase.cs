using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using xLibV100.Ports;

namespace xLibV100.Transceiver
{
    public class RequestBase : IDisposable
    {
        protected xAction<PortResult, byte[]> transmitter;
        protected int tryCount = 1;
        protected int tryNumber = 0;
        protected int responseTimeOut = 100;
        protected int responseTime = 0;
        protected volatile TxStatus status;
        protected volatile TxResponses responseResult;
        protected AutoResetEvent coreSynchronize = new AutoResetEvent(true);
        protected AutoResetEvent txSynchronize = new AutoResetEvent(true);
        protected AutoResetEvent responseSynchronize;
        protected IReceiver receiver;
        protected ResponseHandle handle;

        protected string name;
        protected byte[] data;
        protected Stopwatch stopwatchResponseTime = new Stopwatch();
        protected CancellationToken cancellationToken;
        protected bool isFinished;

        public int ResponseTimeOut => responseTimeOut;
        public int ResponseTime => responseTime;
        public IReceiver GetReceiver() => receiver;
        public TxStatus Status => status;
        public TxResponses ResponseResult => responseResult;

        public ResponseHandle Handle
        {
            get => handle;
            set => handle = value;
        }

        public byte[] Data
        {
            get => data;
            set => data = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public int TryCount
        {
            get => tryCount;
            set
            {
                if (tryCount > 0)
                {
                    tryCount = value;
                }
            }
        }

        public int TryNumber => tryNumber;

        public RequestBase(string name = null, byte[] data = null, int tryCount = 1, int responseTime = 3000)
        {
            this.name = name;
            this.data = data;
            this.tryCount = tryCount;
            responseTimeOut = responseTime;
        }

        public RequestBase()
        {

        }

        public void Accept(TxResponses result)
        {
            try
            {
                coreSynchronize.WaitOne();

                responseResult = result;

                if (status == TxStatus.IsTransmit)
                {
                    status = TxStatus.Complite;
                    responseSynchronize.Set();
                }
            }
            finally
            {
                coreSynchronize.Set();
            }
        }

        public void Accept(ReceiverResult result)
        {
            switch (result)
            {
                case ReceiverResult.NotFound:
                    Accept(TxResponses.NoResponse);
                    break;

                case ReceiverResult.Accept:
                    Accept(TxResponses.Accept);
                    break;

                case ReceiverResult.NotSuported:
                    Accept(TxResponses.NotSuported);
                    break;

                default:
                    break;
            }
        }

        protected void TransmitAction()
        {
            try
            {
                coreSynchronize.WaitOne();

                if (status == TxStatus.IsTransmit)
                {
                    if (tryNumber < tryCount)
                    {
                        if (transmitter == null || transmitter(Data) != PortResult.Accept)
                        {
                            status = TxStatus.TransmiteError;
                            coreSynchronize.Set();
                            responseSynchronize.Set();
                            return;
                        }

                        tryNumber++;
                        stopwatchResponseTime.Restart();
                    }
                    else
                    {
                        status = TxStatus.TimeOut;
                        responseSynchronize.Set();
                    }
                }
            }
            finally
            {
                coreSynchronize.Set();
            }
        }

        protected void Transmit()
        {
            coreSynchronize.WaitOne();

            if (status != TxStatus.Free)
            {
                coreSynchronize.Set();
                return;
            }

            status = TxStatus.Prepare;

            if (Handle != null && !Handle.Add(this))
            {
                status = TxStatus.Busy;
                coreSynchronize.Set();
                return;
            }

            status = TxStatus.IsTransmit;
            responseSynchronize = new AutoResetEvent(false);

            tryNumber = 0;
            responseTime = 0;

            coreSynchronize.Set();

            do
            {
                TransmitAction();

                responseSynchronize.WaitOne(TimeSpan.FromMilliseconds(responseTimeOut));

            } while (status == TxStatus.IsTransmit);

            stopwatchResponseTime.Stop();

            responseTime = (int)stopwatchResponseTime.ElapsedMilliseconds;

            return;
        }

        public virtual void Break()
        {
            coreSynchronize.WaitOne();

            status = TxStatus.Cancelled;
            responseSynchronize?.Set();
            Handle?.Remove(this);

            coreSynchronize.Set();
        }

        /// <summary>
        /// функция отправки асинхронного запроса
        /// </summary>
        /// <param name="port">порт являющийся проводником передоваемого запроса</param>
        /// <param name="tryCount">количество попыток отправки. успешной признается транзакция на которую дан ответ в определенной форме</param>
        /// <param name="responseTimeOut">время ожидания ответа на попытку</param>
        /// <returns></returns>
        public virtual async Task<RequestBase> TransmitAsync(PortBase port, int tryCount, int responseTimeOut)
        {
            txSynchronize.WaitOne();

            if (port == null)
            {
                status = TxStatus.TransmiteActionError;
                goto end;
            }

            isFinished = false;

            this.transmitter = port.Send;
            this.tryCount = tryCount;
            this.responseTimeOut = responseTimeOut;
            this.tryNumber = 0;

            await Task.Run(() => Transmit());

            isFinished = true;

        end:;
            txSynchronize.Set();

            return this;
        }

        /// <summary>
        /// функция отправки асинхронного запроса
        /// </summary>
        /// <param name="port">порт являющийся проводником передоваемого запроса</param>
        /// <param name="tryCount">количество попыток отправки. успешной признается транзакция на которую дан ответ в определенной форме</param>
        /// <param name="responseTimeOut">время ожидания ответа на попытку</param>
        /// <param name="cancellation">токен отмены транзакции</param>
        /// <returns></returns>
        public virtual async Task<RequestBase> TransmitAsync(PortBase port,
            int tryCount,
            int responseTimeOut,
            CancellationToken cancellation)
        {
            txSynchronize.WaitOne();

            if (port == null)
            {
                status = TxStatus.TransmiteActionError;
                goto end;
            }

            isFinished = false;

            this.transmitter = port.Send;
            this.tryCount = tryCount;
            this.responseTimeOut = responseTimeOut;
            this.tryNumber = 0;
            this.cancellationToken = cancellation;

            await Task.Run(() => Transmit(), cancellation);

            isFinished = true;

        end:;
            txSynchronize.Set();

            return this;
        }

        /// <summary>
        /// функция отправки запроса. данная функция забоакирует поток на время ожидания ответа.
        /// максимальное время ожидания = tryCount * responseTimeOut + издержки при работе с потоками.
        /// </summary>
        /// <param name="port">порт являющийся проводником передоваемого запроса</param>
        /// <param name="tryCount">количество попыток отправки. успешной признается транзакция на которую дан ответ в определенной форме</param>
        /// <param name="responseTimeOut">время ожидания ответа на попытку</param>
        /// <returns></returns>
        public virtual RequestBase Transmit(PortBase port,
            int tryCount,
            int responseTimeOut)
        {
            txSynchronize.WaitOne();

            if (port == null)
            {
                status = TxStatus.TransmiteActionError;
                goto end;
            }

            isFinished = false;

            this.transmitter = port.Send;
            this.tryCount = tryCount;
            this.responseTimeOut = responseTimeOut;
            this.tryNumber = 0;

            Transmit();

            isFinished = true;

        end:;
            txSynchronize.Set();

            return this;
        }


        public async Task Await()
        {
            await Task.Run(async () =>
            {
                while (!isFinished)
                {
                    await Task.Delay(1);
                }
            });
        }

        public async Task Await(CancellationToken cancellation)
        {
            await Task.Run(async () =>
            {
                while (!isFinished)
                {
                    await Task.Delay(1);
                }
            }, cancellation);
        }

        public void Dispose()
        {

        }
    }

    public class RequestBase<TResult> : RequestBase
    {
        public TResult Response { get; protected set; }

        public virtual async Task<TResult> AwaitResponseAsync(CancellationToken cancellation = default)
        {
            await Await(cancellation);

            if (ResponseResult != TxResponses.Accept)
            {
                throw new Exception("ResponseResult: error");
            }

            if (Response == null)
            {
                throw new Exception("Response == null");
            }

            return Response;
        }
    }
}