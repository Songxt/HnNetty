using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace HnNetty.Channel
{
    //public class Encoder : MessageToByteEncoder<List<byte>>
    //{
    //    protected override void Encode(IChannelHandlerContext context, List<byte> message, IByteBuffer output)
    //    {
    //        //byte[] messageBytes = Encoding.UTF8.GetBytes(message);
    //        IByteBuffer initialMessage = Unpooled.Buffer(messageBytes.Length);
    //        initialMessage.WriteBytes(messageBytes);
    //        output.WriteBytes(message);
    //    }
    //}

    //public class ServerDecoder : ByteToMessageDecoder
    //{
    //    protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
    //    {
    //        byte[] array = new byte[input.ReadableBytes];
    //        input.GetBytes(input.ReaderIndex, array, 0, input.ReadableBytes);
    //        input.Clear();

    //        var msg = new Message();
    //        output.Add(msg.Prase(array));
    //    }
    //}
}