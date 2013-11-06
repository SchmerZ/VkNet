using VkSync.Mediators;

namespace VkSync.Models
{
    public class MediatorViewModel : BindingModel
    {
        public Mediator<ViewModelMessageType> Mediator
        {
            get
            {
                return Mediator<ViewModelMessageType>.Instance;
            }
        } 
    }
}