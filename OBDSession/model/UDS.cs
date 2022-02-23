namespace TaycanLogger
{
    public partial class OBD2
    {
        public OBD2Command[] init;
        public OBD2Rotation rotation;
    }

  
    public partial class OBD2Command
    {
        public string send;
    }

   
    public  class OBD2Rotation
    {
        public OBD2RotationCommand[] command;
        public string[] Text;
    }


    public class OBD2RotationCommand
    {

        public OBD2RotationCommandValue[] values;
        public string send;
        public int skipCount;
        public string name;
        public string conversion;
        public string units;
        public byte targetId;
        public bool targetIdSpecified;
    }

    public partial class OBD2RotationCommandValue
    {

        private string nameField;

        private string unitsField;

        private string conversionField;

        private byte targetIdField;

        private bool targetIdFieldSpecified;

        private byte offsetField;

        private bool offsetFieldSpecified;

        private byte lengthField;

        private bool lengthFieldSpecified;

        private string enumField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string units
        {
            get
            {
                return this.unitsField;
            }
            set
            {
                this.unitsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string conversion
        {
            get
            {
                return this.conversionField;
            }
            set
            {
                this.conversionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte targetId
        {
            get
            {
                return this.targetIdField;
            }
            set
            {
                this.targetIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool targetIdSpecified
        {
            get
            {
                return this.targetIdFieldSpecified;
            }
            set
            {
                this.targetIdFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte offset
        {
            get
            {
                return this.offsetField;
            }
            set
            {
                this.offsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool offsetSpecified
        {
            get
            {
                return this.offsetFieldSpecified;
            }
            set
            {
                this.offsetFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte length
        {
            get
            {
                return this.lengthField;
            }
            set
            {
                this.lengthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lengthSpecified
        {
            get
            {
                return this.lengthFieldSpecified;
            }
            set
            {
                this.lengthFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @enum
        {
            get
            {
                return this.enumField;
            }
            set
            {
                this.enumField = value;
            }
        }
    }



}
