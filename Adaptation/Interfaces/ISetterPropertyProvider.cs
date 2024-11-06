namespace xLibV100.Adaptation
{
    public interface ISetterPropertyProvider
    {
        PropertyAdaptionMode AdaptionMode { get; }

        PropertyAdaptionFlags Flags { get; }

        byte[] ComposeRequest();
    }
}
