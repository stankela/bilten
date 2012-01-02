using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten
{
    public class NotificationMessage : IComparable
    {
        private string _fieldName;
        private string _message;

        public NotificationMessage(string fieldName, string message)
        {
            _fieldName = fieldName;
            _message = message;
        }

        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        // Override the Equals method to make declarative testing easy
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            NotificationMessage notificationMessage = obj as NotificationMessage;
            if (notificationMessage == null) return false;
            return Equals(_fieldName, notificationMessage._fieldName)
                && Equals(_message, notificationMessage._message);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 14;
                result = 29 * result + FieldName.GetHashCode();
                result = 29 * result + Message.GetHashCode();
                return result;
            }
        }

        // Override the ToString() method to create more descriptive messages
        // within xUnit testing assertions
        public override string ToString()
        {
            return string.Format("Field {0}:  {1}", _fieldName, _message);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is NotificationMessage)
            {
                NotificationMessage other = (NotificationMessage)obj;

                int result = _fieldName.CompareTo(other._fieldName);
                if (result != 0)
                    return result;
                else
                    return _message.CompareTo(other._message);
            }

            throw new ArgumentException("object is not a NotificationMessage");
        }

        #endregion
    }
}
