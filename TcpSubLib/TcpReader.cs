using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text.Json;

namespace TcpSubLib
{
    /// <summary>
    /// Uses a TCP subscriber model to read data from a <see cref="TcpConnector"/>.
    /// </summary>
    public class TcpReader : IDisposable
    {
        public int AvailableData
        {
            get
            {
                return _client.Available;
            }
        }

        private readonly TcpClient _client;

        /// <summary>
        /// Initializes a new <see cref="TcpReader"/>.
        /// </summary>
        /// <param name="address">
        /// The address to connect to.
        /// Use <see cref="IPAddress.Any"/> to connect to the local address.
        /// </param>
        /// <param name="port">The port to connect to.</param>
        public TcpReader(IPAddress address, ushort port)
        {
            _client = new TcpClient();
            _client.Connect(address, port);
        }

        /// <summary>
        /// Reads bytes from the underlying connection.
        /// The first two read bytes will determine the size of the returned amount of bytes.
        /// </summary>
        /// <returns>Returns the read data.</returns>
        public byte[] Read()
        {
            byte[] length = new byte[sizeof(ushort)];
            _client.GetStream().Read(length);
            ushort lengthShort = BitConverter.ToUInt16(length);

            return Read(lengthShort);
        }

        /// <summary>
        /// Reads bytes from the underlying connection.
        /// </summary>
        /// <param name="length">The amount of bytes that should be read.</param>
        /// <returns>Returns the read data.</returns>
        public byte[] Read(ushort length)
        {
            byte[] readData = new byte[length];
            _client.GetStream().Read(readData);

            return readData;
        }

        /// <summary>
        /// Reads bytes from the underlying connection and deserializes it.
        /// The first two read bytes will determine the size of the returned amount of bytes.
        /// </summary>
        /// <returns>Returns the read data.</returns>
        public T Read<T>()
        {
            if (!typeof(T).IsSerializable)
                throw new ArgumentException($"Type {nameof(T)} must be serializable.");

            return JsonSerializer.Deserialize<T>(Read());
        }

        /// <summary>
        /// Reads bytes asynchronously from the underlying connection.
        /// The first two read bytes will determine the size of the returned amount of bytes.
        /// </summary>
        /// <returns>Returns the read data.</returns>
        public async Task<byte[]> ReadAsync()
        {
            byte[] length = new byte[sizeof(ushort)];
            await _client.GetStream().ReadAsync(length);
            ushort lengthShort = BitConverter.ToUInt16(length);

            return await ReadAsync(lengthShort);
        }

        /// <summary>
        /// Reads bytes asynchronously from the underlying connection.
        /// </summary>
        /// <param name="length">The amount of bytes that should be read.</param>
        /// <returns>Returns the read data.</returns>
        public async Task<byte[]> ReadAsync(ushort length)
        {
            byte[] readData = new byte[length];
            await _client.GetStream().ReadAsync(readData);

            return readData;
        }

        /// <summary>
        /// Reads bytes asynchronously from the underlying connection and deserializes it.
        /// The first two read bytes will determine the size of the returned amount of bytes.
        /// </summary>
        /// <returns>Returns the read data.</returns>
        public async Task<T> ReadAsync<T>()
        {
            if (!typeof(T).IsSerializable)
                throw new ArgumentException($"Type {nameof(T)} must be serializable.");

            return JsonSerializer.Deserialize<T>(await ReadAsync());
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}