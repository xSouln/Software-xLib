namespace xLibV100.Adaptation
{
    public interface IGetterPropertyProvider
    {
        PropertyAdaptionMode AdaptionMode { get; }

        PropertyAdaptionFlags Flags { get; }

        byte[] ComposeRequest();
    }
}
