namespace Trading.Common.ViewModels
{
    public class VolumeValueViewModel : ViewModelBaseSlim
    {
        private decimal _value;
        private long volume;

        public VolumeValueViewModel(decimal value, long volume)
        {
            Value = value;
            Volume = volume;
        }

        public decimal Value
        {
            get { return _value; }
            set { SetNotify(ref _value, value); }
        }

        public long Volume
        {
            get { return volume; }
            set { SetNotify(ref volume, value); }
        }
    }
}