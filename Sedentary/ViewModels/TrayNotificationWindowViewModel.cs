namespace Sedentary.ViewModels
{
    public class TrayNotificationWindowViewModel
    {
        private readonly object _contentModel;

        public TrayNotificationWindowViewModel(object contentModel)
        {
            _contentModel = contentModel;
        }

        public object ContentModel
        {
            get { return _contentModel; }
        }
    }
}