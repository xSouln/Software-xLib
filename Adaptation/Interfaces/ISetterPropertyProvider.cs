namespace xLibV100.Adaptation
{
    public interface ISetterPropertyProvider
    {
        PropertyAdaptionMode AdaptionMode { get; }

        SetterPropertyAdaptionFlags Flags { get; }

        byte[] ComposeRequest();
    }
}
