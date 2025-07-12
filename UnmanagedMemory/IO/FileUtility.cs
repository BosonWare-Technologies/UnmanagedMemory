using System.Buffers;
using System.Text;

namespace UnmanagedMemory.IO;

public static class FileUtility
{
    public static async Task<string> ReadAllTextAsync(string path)
    {
        using var bytes = await ReadAllBytesAsync(path);

        unsafe {
            return Encoding.UTF8.GetString(bytes.AsUnsafePointer(), bytes.Length);
        }
    }

    public static async Task<UnsafeMemory<byte>> ReadAllBytesAsync(string path)
    {
        const int bufferSize = 4096;

        using var stream = File.OpenRead(path);

        int length = (int)stream.Length;

        var memory = new UnsafeMemory<byte>(length);

        var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

        try {
            int position = 0;
            while (true) {
                int bytesRead = await stream.ReadAsync(buffer);

                memory.Write(buffer.AsSpan(0, bytesRead), position);

                position += bytesRead;

                if (bytesRead < buffer.Length) {
                    break;
                }
            }
        }
        catch {
            memory.Dispose();

            throw;
        }
        finally {
            buffer.AsSpan().Clear();
            ArrayPool<byte>.Shared.Return(buffer);
        }

        return memory;
    }
}