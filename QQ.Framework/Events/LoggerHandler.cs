using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ.Framework.Events
{
    public interface LoggerHandler
    {
        void MessageLog(string msg, long fromQQ, MsgType type);
    }
}
