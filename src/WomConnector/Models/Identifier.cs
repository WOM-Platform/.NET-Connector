using System;

namespace WomPlatform.Connector.Models {

    public struct Identifier {

        public string Id;

        public Identifier(string id) {
            Id = id;
        }

        public Identifier(long id) {
            if(id < 0) {
                throw new ArgumentOutOfRangeException();
            }

            Id = id.ToString();
        }

        public static implicit operator string(Identifier vId) => vId.Id;

        public static implicit operator Identifier(string sId) => new Identifier(sId);

        public static implicit operator Identifier(long id) => new Identifier(id);

        public override bool Equals(object obj) {
            return Id.Equals(obj);
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        public override string ToString() {
            return Id;
        }

    }

}
