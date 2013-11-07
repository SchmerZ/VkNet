using VkSync.Mediators;

namespace VkSync.Models
{
    public class MediatorViewModel : BindingModel
    {
        protected Mediator<ViewModelMessageType> Mediator
        {
            get
            {
                return Mediator<ViewModelMessageType>.Instance;
            }
        }
    }
}