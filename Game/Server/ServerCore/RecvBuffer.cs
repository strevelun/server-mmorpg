using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
	public class RecvBuffer : Producer
	{
		// TODO: TLS (Thread Local Storage) 로 변경해야함.
		internal static ConcurrentQueue<RecvBuffer> Pool = new ConcurrentQueue<RecvBuffer> ();
		internal static int BufferSize;
		public static void InitPool(int bufferCount, int bufferSize)
        {
			BufferSize = bufferSize;
			for (int i = 0; i < bufferCount; ++i)
            {
				Pool.Enqueue(new RecvBuffer(bufferSize));
            }
        }

		public static RecvBuffer Pop()
        {
			return Pool.TryDequeue(out var buffer) == true ? buffer : new RecvBuffer(BufferSize);
        }

		public static void Push(RecvBuffer buffer)
        {
			buffer.Clean();
			Pool.Enqueue(buffer);
        }

		// [r][][w][][][][][][][]
		ArraySegment<byte> _buffer;
		int _readPos;
		int _writePos;
		Session _session = null;

		public Session Session { get { return _session; } }

		public RecvBuffer(int bufferSize)
		{
			_buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
		}

		public int DataSize { get { return _writePos - _readPos; } }
		public int FreeSize { get { return _buffer.Count - _writePos; } }

		public ArraySegment<byte> ReadSegment
		{
			get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
		}

		public ArraySegment<byte> WriteSegment
		{
			get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
		}

		public void Copy(ArraySegment<byte> segment, int offset, int size)
        {
			Buffer.BlockCopy(segment.Array, offset, _buffer.Array, _buffer.Offset, size);
        }

		public void Clean()
		{
			int dataSize = DataSize;
			if (dataSize == 0)
			{
				// 남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
				_readPos = _writePos = 0;
			}
			else
			{
				// 남은 찌끄레기가 있으면 시작 위치로 복사
				Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
				_readPos = 0;
				_writePos = dataSize;
			}

			_session = null;
		}

		public bool OnRead(int numOfBytes)
		{
			if (numOfBytes > DataSize)
				return false;

			_readPos += numOfBytes;
			return true;
		}

		public bool OnWrite(int numOfBytes)
		{
			if (numOfBytes > FreeSize)
				return false;

			_writePos += numOfBytes;
			return true;
		}

		public void SetSession(Session _session)
        {
			this._session = _session;
        }
	}
}
