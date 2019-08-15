using QQ.Framework.Packets;

namespace QQ.Framework.Domains
{
    /// <summary>
    ///     连接服务
    /// </summary>
    public interface ISocketService
    {
        /// <summary>
        ///     接收来自服务器的消息
        /// </summary>
        /// <returns></returns>
        ReceiveData Receive();

        /// <summary>
        ///     发送消息
        /// </summary>
        void Send(SendPacket packet);

        /// <summary>
        ///     刷新服务器地址
        /// </summary>
        /// <param name="host"></param>
        void RefreshHost(string host);

        /// <summary>
        ///     记录日志
        /// </summary>
        /// <param name="content">内容</param>
        void MessageLog(string content,MsgType type);

        /// <summary>
        ///     登录
        /// </summary>
        void Login();

        /// <summary>
        ///     处理登录结果
        /// </summary>
        /// <param name="isSuccess">是否成功</param>
        /// <param name="message">消息</param>
        void LoginCallback(bool isSuccess, string message);

        /// <summary>
        ///     接收验证码
        /// </summary>
        /// <param name="data"></param>
        void ReceiveVerifyCode(byte[] data);
    }
}
