using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Domain.Base
{
    public abstract class DBObject<T>
    {

        DateTime _dateCreated = DateTime.Now;
        DateTime _dateModified = DateTime.Now;
        bool _isDeleted = false;

        [Key]
        public T Id { get; set; }

        public virtual string UserId { get; set; }

        public virtual DateTime? DateCreated { get => _dateCreated; set => _dateCreated = value.Value; }

        public virtual DateTime? DateModified { get => _dateModified; set => _dateModified = value.Value; }

        public virtual string ModifiedUserId { get; set; }

        public virtual bool IsDeleted { get => _isDeleted; set => _isDeleted = value; }

    }

    public abstract class DBObject : DBObject<int>
    {

    }

   


}
