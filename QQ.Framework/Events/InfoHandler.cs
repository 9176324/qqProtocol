using QQ.Framework.HttpEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ.Framework.Events
{
    public interface InfoHandler
    {
        void GetFriendAndGroup(long fromQQ,FriendList friends,GroupList groups);
    }
}
