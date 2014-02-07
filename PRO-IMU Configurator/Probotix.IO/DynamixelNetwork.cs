using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;

namespace Probotix.IO
{
    public class DynamixelNetwork
    {
        /// <summary>
        /// The types of instructions that can be sent to Dynamixels using WriteInstruction.
        /// </summary>
        /// <seealso cref="WriteInstruction"/>
        public enum Instruction : byte
        {
            /// <summary>
            /// Respond only with a status packet.
            /// </summary>
            Ping = 1,
            /// <summary>
            /// Read register data.
            /// </summary>
            ReadData = 2,
            /// <summary>
            /// Write register data.
            /// </summary>
            WriteData = 3,
            /// <summary>
            /// Delay writing register data
            /// until an Action instruction is received.
            /// </summary>
            RegWrite = 4,
            /// <summary>
            /// Perform pending RegWrite instructions.
            /// </summary>
            Action = 5,
            /// <summary>
            /// Reset all registers (including ID) to default values.
            /// </summary>
            Reset = 6,
            /// <summary>
            /// Write register data to multiple Dynamixels at once.
            /// </summary>
            SyncWrite = 0x83,
        }

        [Flags]
        public enum ErrorStatus : byte
        {
            /// <summary>
            /// Input Voltage Error
            /// </summary>
            [Description("Input Voltage Error")]
            InputVoltage = 1,
            /// <summary>
            /// Angle Limit Error
            /// </summary>
            [Description("Angle Limit Error")]
            AngleLimit = 2,
            /// <summary>
            /// Overheating Error
            /// </summary>
            [Description("Overheating Error")]
            Overheating = 4,
            /// <summary>
            /// Range Error
            /// </summary>
            [Description("Range Error")]
            Range = 8,
            /// <summary>
            /// Checksum Error
            /// </summary>
            [Description("Checksum Error")]
            Checksum = 0x10,
            /// <summary>
            /// Overload Error
            /// </summary>
            [Description("Overload Error")]
            Overload = 0x20,
            /// <summary>
            /// Instruction Error
            /// </summary>
            [Description("Instruction Error")]
            Instruction = 0x40,
        }

        public const int BroadcastId = 254;
        private object _key;
        private SerialPort Stream
        {
            set;
            get;
        }


        public bool IsOpen
        {
            protected set;
            get;
        }

        public DynamixelNetwork(string portName, int baudNum)
        {
            _key = new object();
            Stream = new SerialPort(portName)
            {
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                ReadBufferSize = 2048,
                WriteBufferSize = 2048,
                ReadTimeout = 40,
                WriteTimeout = 20
            };
            BaudNum = baudNum;
        }

        private int _baudnum;
        public int BaudNum
        {
            get
            {
                return _baudnum;
            }
            set
            {
                lock (_key)
                {
                    _baudnum = value;
                    Stream.BaudRate = 2000000 / (value + 1);
                }
            }
        }

        public int BaudRate
        {
            get
            {
                lock (_key)
                {
                    return Stream.BaudRate;
                }
            }
            set
            {
                lock (_key)
                {
                    Stream.BaudRate = value;
                    _baudnum = (2000000 / value) - 1;
                }
            }
        }

        public bool Open()
        {
            try
            {
                lock (_key)
                {
                    Stream.Open();
                    IsOpen = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                IsOpen = false;
            }
            return IsOpen;
        }

        public bool Close()
        {
            try
            {
                lock (_key)
                {
                    Stream.Close();
                    IsOpen = false;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                IsOpen = true;
            }
            return !IsOpen;
        }

        public byte[] ReadPacket(out int id)
        {
            lock (_key)
            {
                id = 0xFF; // set an invalid id for error return cases

                var inputByte = Stream.ReadByte(); // 1st header byte
                if (inputByte != 0xFF)
                {
                    return null;
                }

                inputByte = Stream.ReadByte(); // 2nd header byte
                if (inputByte != 0xFF)
                {
                    return null;
                }

                inputByte = Stream.ReadByte();
                if (inputByte == 0xFF) // have seen a third header byte! not sure why/how
                {
                    inputByte = Stream.ReadByte();
                }

                id = inputByte;
                var length = Stream.ReadByte() - 2;
                if (length < 0)
                {
                    return null;
                }
                var error = Stream.ReadByte(); // TODO: Do something on Error
                byte[] data = null;

                if (length > 0)
                {
                    // read the data, if any
                    data = new byte[length];
                    int offset = 0;
                    while (length > 0)
                    {
                        int count = Stream.Read(data, offset, length);
                        length -= count;
                        offset += count;
                    }
                }

                var checkSum = Stream.ReadByte(); // TODO: Could validate the checksum and reject the packet.

                return data;

            }
        }

        private byte[] _data;
        int _packetId;
        private int _packetLength;
        public byte[] ReadPacket(int id, int length)
        {
            // never expect a return packet from a broadcast instruction.

            if (id == BroadcastId)
                return null;
            do
            {

                _data = ReadPacket(out _packetId);
                _packetLength = _data == null ? 0 : _data.Length;
                if (_packetId == id && _packetLength == length)
                    return _data;	// this is the normal return case

            } while (true);
        }

        public int ReadByte(int id, int address)
        {
            byte[] data = ReadData(id, (byte)address, 1);
            return data[0];
        }

        public int ReadWord(int id, int address)
        {
            byte[] data = ReadData(id, (byte)address, 2);
            return (data[1] << 8) + data[0];
        }

        public void WriteInstruction(int id, Instruction instruction, List<byte> parms)
        {
            lock (_key)
            {
                // command packet sent to Dynamixel servo:
                // [0xFF] [0xFF] [id] [length] [...data...] [checksum]
                var instructionPacket = new List<byte>
                           {
                               0xff,
                               0xff,
                               (byte) id,
                               (byte) (((parms != null) ? parms.Count : 0) + 2), // length is the data-length + 2
                               (byte)instruction
                           };

                if (parms != null && parms.Count != 0)
                    instructionPacket.AddRange(parms);

                var cheksum = 0;
                for (var i = 2; i < instructionPacket.Count; i++)
                {
                    cheksum += instructionPacket[i];
                }
                instructionPacket.Add((byte)(~(cheksum & 0xff)));

                Stream.Write(instructionPacket.ToArray(), 0, instructionPacket.Count);
            }
        }

        public byte[] ReadData(int id, byte startAddress, int count)
        {
            var parameters = new List<byte> { startAddress, (byte)count };
            WriteInstruction(id, Instruction.ReadData, parameters);
            return ReadPacket(id, count);
        }

        public void WriteData(int id, int startAddress, List<byte> values, bool flush)
        {
            try
            {
                var writePacket = new List<byte> {(byte) startAddress};
                writePacket.AddRange(values);
                WriteInstruction(id, flush ? Instruction.WriteData : Instruction.RegWrite, writePacket);
                if (flush)
                    ReadPacket(id, 0);
            }
                      
            catch (Exception)
            {
            
            }
        }

        public void WriteByte(int id, int registerAddress, int value, bool flush)
        {
            WriteData(id, registerAddress, new List<byte> { (byte)value }, flush);
        }

        public void WriteWord(int id, int registerAddress, int value, bool flush)
        {
            WriteData(id, registerAddress, new List<byte> { (byte)(value & 0xFF), (byte)(value >> 8) }, flush);
        }

        public void SyncWrite(byte startAddress, int numberOfDynamixels, List<byte> parms)
        {
            // Todo : To be implemented soon ! 
        }

        public List<int> ScanIds(int startId, int endId)
        {
            if (endId > 253 || endId < 0)
                throw new ArgumentException("endID must be 0 to 253");
            if (startId > endId || startId < 0)
                throw new OverflowException("startID must be 0 to endID");

            var ids = new List<int>();
            for (int id = startId; id <= endId; id++)
            {
                if (Ping(id))
                    ids.Add(id);
            }
            return ids;
        }

        public void Action()
        {
            WriteInstruction(BroadcastId, Instruction.Action, null);
        }

        public bool Ping(int id)
        {
            WriteInstruction(id, Instruction.Ping, null);
            try
            {
                ReadPacket(id, 0);
            }
            catch (TimeoutException)
            {
                return false;
            }
            return true;
        }

    }
}
