using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using QQ.Framework.Packets.PCTLV;
using QQ.Framework.TlvLib;
using QQ.Framework.Utils;

namespace QQ.Framework.Packets
{
    public class ReceivePacket : Packet
    {
        public BinaryReader Reader;

        public ReceivePacket()
        {
        }

        /// <summary>
        ///     构造一个指定参数的包
        /// </summary>
        /// <param name="byteBuffer"></param>
        /// <param name="user"></param>
        /// <param name="key">解密Key</param>
        public ReceivePacket(byte[] byteBuffer, QQUser user, byte[] key)
            : base(byteBuffer, user)
        {
            Reader = new BinaryReader(new MemoryStream(Buffer));
            BodyEcrypted = byteBuffer;
            //指定随包解密Key
            SecretKey = key;
            //提取包头部分
            ParseHeader();

            try
            {
                //解析包
                ParseBody();
            }
            catch (Exception e)
            {
                User.MessageLog($"包内容解析出错,错误{e.Message}，包名: {ToString()}",MsgType.ERROR);
            }

            //提取包尾部分
            ParseTail();
        }

        public long QQ { get; set; }

        public byte[] Decrypt(byte[] key)
        {
            BodyDecrypted = QQTea.Decrypt(Buffer, (int) Reader.BaseStream.Position,
                (int) (Buffer.Length - Reader.BaseStream.Position - 1), key);
            if (BodyDecrypted == null)
            {
                throw new Exception($"包内容解析出错，抛弃该包: {ToString()}");
            }

            Reader = new BinaryReader(new MemoryStream(BodyDecrypted));
            return Reader.ReadRest();
        }

        /// <summary>
        ///     包体长度
        /// </summary>
        /// <returns></returns>
        public int GetPacketLength()
        {
            return BodyEcrypted.Length;
        }

        /// <summary>
        ///     从buf的当前位置解析包尾
        /// </summary>
        protected void ParseTail()
        {
            try
            {
                Reader.ReadByte();
            }
            catch
            {
            }
        }

        /// <summary>
        ///     解析包体，从buf的开头位置解析起
        /// </summary>
        protected virtual void ParseBody()
        {
        }

        /// <summary>
        ///     从buf的当前位置解析包头
        /// </summary>
        protected virtual void ParseHeader()
        {
            Header = Reader.ReadByte();
            Version = Reader.BeReadUInt16();
            Command = (QQCommand) Reader.BeReadUInt16();
            Sequence = Reader.BeReadUInt16();
            QQ = Reader.BeReadInt32();
            Reader.ReadBytes(3);
        }

        public void GetImage(string fileName)
        {
            var api =
                "https://gchat.qpic.cn/gchatpic_new/807977219/485750189-2603962136-64ECA8CA06FC5B0CE6F047FEB66768B0/0?vuin=417085811&term=2addtime=1515123740";
        }

        private Dictionary<int, Type> _tlvTypes;
        /// <summary>
        ///     通过反射执行TLV返回包解析
        /// </summary>
        /// <param name="tlvs"></param>
        internal void TlvExecutionProcessing(ICollection<Tlv> tlvs)
        {
            if (_tlvTypes == null)
            {
                var types = Assembly.GetExecutingAssembly().GetTypes();
                _tlvTypes = new Dictionary<int, Type>();
                foreach (var type in types)
                {
                    var attributes = type.GetCustomAttributes();
                    if (!attributes.Any(attr => attr is TlvTagAttribute))
                    {
                        continue;
                    }

                    var attribute = attributes.First(attr => attr is TlvTagAttribute) as TlvTagAttribute;
                    _tlvTypes.Add((int) attribute.Tag, type);
                }
            }
            foreach (var tlv in tlvs)
            {
                if (_tlvTypes.ContainsKey(tlv.Tag))
                {
                    var tlvClass = Assembly.GetExecutingAssembly().CreateInstance(_tlvTypes[tlv.Tag].FullName, true);
                    var methodinfo = _tlvTypes[tlv.Tag].GetMethod("Parser_Tlv");
                    methodinfo.Invoke(tlvClass, new object[] { User, Reader });
                }
            }
        }
    }
}