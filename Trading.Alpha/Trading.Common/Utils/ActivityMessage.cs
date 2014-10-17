namespace Trading.Common.Utils
{
    public class ActivityMessage<T>
    {
        public ActivityMessage(ActivityType type, string screen = "", string callerName = "")
        {
            Type = type;
            ScreenName = screen;
            CallerName = callerName;
        }

        public ActivityMessage(ActivityType type, object item)
            : this(type)
        {
            Item = item;
        }

        /// <summary>
        /// Get/set the type of action done.
        /// </summary>
        public ActivityType Type { get; set; }
        /// <summary>
        /// Get/set the screen this message belongs to. It is optional.
        /// </summary>
        public string ScreenName { get; set; }
        /// <summary>
        /// Get/set the caller view/screen's name. It is optional.
        /// </summary>
        public string CallerName { get; set; }
        /// <summary>
        /// Get/set the additional object parameter. It is optional.
        /// </summary>
        public object Item { get; set; }

        #region equality members

        protected bool Equals(ActivityMessage<T> other)
        {
            return Type == other.Type && string.Equals(ScreenName, other.ScreenName) && Equals(Item, other.Item);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ActivityMessage<T>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)Type;
                hashCode = (hashCode * 397) ^ (ScreenName != null ? ScreenName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Item != null ? Item.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ActivityMessage<T> left, ActivityMessage<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ActivityMessage<T> left, ActivityMessage<T> right)
        {
            return !Equals(left, right);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Type: {0}, Item: {1}", Type, Item);
        }
    }
}