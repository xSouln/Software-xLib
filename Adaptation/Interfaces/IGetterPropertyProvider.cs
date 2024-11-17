namespace xLibV100.Adaptation
{
    public interface IGetterPropertyProvider
    {
        PropertyAdaptionMode AdaptionMode { get; }

        GetterPropertyAdaptionFlags Flags { get; }

        byte[] ComposeRequest();
    }
}
