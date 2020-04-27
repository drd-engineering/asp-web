    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models
{

    public class UserInboxData
    {
        public string EncryptedUserId { get; set; }
        public long Id { get; set; } // Id (Primary key)
        public string EncryptedId { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 50)
        public string Phone { get; set; } // Phone (length: 20)
        public string Email { get; set; } // Email (length: 50)
        public long OfficialIdNo { get; set; }
        public string ImageProfile { get; set; }
        public string Password { get; set; } // Password (length: 20)
        public string ImageSignature { get; set; }

        public string ImageInitials { get; set; }

        public string ImageStamp { get; set; }

        public string ImageKtp1 { get; set; }

        public string ImageKtp2 { get; set; }
        public bool IsActive { get; set; } // IsActive
        public DateTime CreatedAt { get; set; } // DateCreated

        public UserInboxData() {
            Id = RandomLongGenerator(minimumValue: 1000000000, maximumValue: 10000000000);
        }

        private long RandomLongGenerator(long minimumValue, long maximumValue)
        {
            Random randomClass = new Random();
            byte[] buf = new byte[8];
            randomClass.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (maximumValue - minimumValue)) + minimumValue);
        }
    }
}
