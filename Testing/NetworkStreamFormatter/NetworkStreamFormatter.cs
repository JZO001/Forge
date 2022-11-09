using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Forge.Persistence.Formatters;

namespace Forge.Testing.NetworkStreamFormatter
{
    [Serializable]
    public class MyData
    {
        #region Fields

        public string sendData;

        public string message;

        public byte[] data = null;

        public int bufferSize = 0;

        #endregion

        #region Constructors

        public MyData()
        {
            this.bufferSize = 0;
            this.sendData = null;
            this.message = null;
        }

        public MyData(byte[] data)
        {
            int lenghtSendData = BitConverter.ToInt32(data, 0);
            int lenghtMessage = BitConverter.ToInt32(data, 4);
            if (lenghtSendData > 0)
            {
                this.sendData = Encoding.UTF8.GetString(data, 12, lenghtSendData);
            }
            else
            {
                this.sendData = null;
            }

            if (lenghtMessage > 0)
            {
                this.message = Encoding.UTF8.GetString(data, 12, lenghtMessage);
            }
            else
            {
                this.message = null;
            }
        }

        #endregion

        public byte[] ToByte()
        {
            List<byte> result = new List<byte>();
            if (sendData != null)
            {
                result.AddRange(BitConverter.GetBytes(sendData.Length));
            }
            else
            {
                result.AddRange(BitConverter.GetBytes(0));
            }

            if (message != null)
            {
                result.AddRange(BitConverter.GetBytes(message.Length));
            }
            else
            {
                result.AddRange(BitConverter.GetBytes(0));
            }

            if (sendData != null)
            {
                result.AddRange(Encoding.UTF8.GetBytes(sendData));
            }

            if (message != null)
            {
                result.AddRange(Encoding.UTF8.GetBytes(message));
            }

            return result.ToArray();
        }
    }

    class NetworkStreamFormatter : IDataFormatter<MyData>
    {
        #region Fields

        private BinaryFormatter mFormatter = new BinaryFormatter();

        private BinaryFormatter<MyData> formatter = new BinaryFormatter<MyData>();

        #endregion

        #region Construcotr

        public NetworkStreamFormatter()
        {
        }

        #endregion

        #region Implement Methods

        public bool CanRead(System.IO.Stream stream)
        {
            bool result = false;
            bool restore = false;
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            long position = 0;
            try
            {
                position = stream.Position;
                restore = true;
                result = true;
            }
            catch (Exception)
            {
                try
                {
                    if (stream.Length > 0)
                    {
                        result = true;
                    }
                }
                catch (Exception)
                {
                }
            }

            if (result)
            {
                try
                {
                    this.mFormatter.Deserialize(stream);
                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                }
                finally
                {
                    if (restore)
                    {
                        stream.Position = position;
                    }
                }
            }
            return result;
        }

        public bool CanWrite(MyData item)
        {
            bool result = false;
            bool restore = false;

            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            long position = 0;
            
            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    position = ms.Position;
                    result = true;
                    restore = true;
                }
                catch (Exception)
                {
                    try
                    {
                        if(ms.Length > 0)
                        {
                            result = true;
                        }
                    }
                    catch (Exception)
                    { 
                    }
                }

                if (result)
                {
                    try
                    {
                        this.mFormatter.Serialize(ms, item);
                        result = true;
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                    finally
                    {
                        if(restore)
                        {
                            ms.Position = position;
                        }
                    }
                }
            }
            return result;
        }

        public MyData Read(System.IO.Stream stream)
        {
            int headerLenghtint = 0;
            string headerLenghtString = null;
            MyData result = null;

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            try
            {
                result = (MyData)this.mFormatter.Deserialize(stream);
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FormatException(ex.Message, ex);
            }

            //Header hossz elkérése
            using (MemoryStream ms = new MemoryStream())
            {
                //beleírom a tempByte-be a stream első bájtját.
                int tempByte = stream.ReadByte();
                if (tempByte == 0)
                {
                    throw new ProtocolViolationException("The stream length is 0");
                }
                //amig a streamba nincs 0-a addig az első byte-t belewritelem az ms memoryStreamba.
                while (tempByte > 0)
                {
                    ms.WriteByte((byte)tempByte);
                    tempByte = stream.ReadByte();
                }

                headerLenghtString = Encoding.UTF8.GetString(ms.ToArray());

                try
                {
                    headerLenghtint = int.Parse(headerLenghtString);
                }
                catch (Exception ex)
                {
                    throw new ProtocolViolationException(ex.Message);
                }
                if (headerLenghtint == 0)
                {
                    throw new ProtocolViolationException("The header length is 0");
                }

                byte[] data = new byte[headerLenghtint];

                ReadStream(stream, data);

                MyData header = null;
                using (MemoryStream input = new MemoryStream(data))
                {
                    header = this.formatter.Read(input);
                }

                if (header == null)
                {
                    throw new Exception("The header is null");
                }
                else
                {
                    result = header;  
                }
            }
            return result;
        }

        public void Write(System.IO.Stream stream, MyData data)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            try
            {
                this.mFormatter.Serialize(stream, data);
            }
            catch (Exception)
            {
                throw;
            }

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Write(ms, data);
                    byte[] headerBytes = Encoding.UTF8.GetBytes(ms.Length.ToString());
                    using (MemoryStream msOut = new MemoryStream())
                    {
                        msOut.Write(headerBytes, 0, headerBytes.Length);
                        msOut.WriteByte((byte)0);
                        ms.WriteTo(msOut);
                        msOut.WriteTo(stream);
                    }
                }
            }
            catch (FormatException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new FormatException(ex.Message, ex);
            }

        }
        
        public static void ReadStream(Stream stream, byte [] data)
        {
            int readByte = 0;
            while (readByte < data.Length)
            {
                readByte += stream.Read(data, readByte, data.Length - readByte);
            }
        }

        public object Clone()
        {
            return new NetworkStreamFormatter();
        }

        #endregion
    }
}
