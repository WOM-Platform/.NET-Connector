using System;

namespace WomPlatform.Connector.Models {

    public struct VoucherId {

        public string Id;

        public VoucherId(string id) {
            Id = id;
        }

        public VoucherId(long id) {
            if(id < 0) {
                throw new ArgumentOutOfRangeException();
            }

            Id = id.ToString();
        }

        public static implicit operator string(VoucherId vId) => vId.Id;

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
