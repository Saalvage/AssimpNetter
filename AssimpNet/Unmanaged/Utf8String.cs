using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Assimp.Unmanaged;

/// <summary>
/// NetStandard 2.0 does not have built-in support for marshalling UTF-8 strings.
/// </summary>
public sealed class Utf8String(IntPtr ptr) : IDisposable
{
    private bool m_disposed;

    /// <summary>
    /// Gets the unmanaged pointer to the UTF-8 encoded string.
    /// </summary>
    public IntPtr Pointer { get; private set; } = ptr;

    /// <summary>
    /// Creates a new UTF-8 encoded string from a managed string. 
    /// </summary>
    public static Utf8String From(string text)
    {
        if (text == null) return new Utf8String(IntPtr.Zero);

        byte[] bytes = Encoding.UTF8.GetBytes(text);
        IntPtr ptr = Marshal.AllocHGlobal(bytes.Length + 1);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        Marshal.WriteByte(ptr, bytes.Length, 0); // trailing '\0'
        return new Utf8String(ptr);
    }

    /// <summary>Implicit conversion between Utf8String and IntPtr</summary>
    public static implicit operator IntPtr(Utf8String s) => s?.Pointer ?? IntPtr.Zero;

    /// <summary>Cleanup IntPtr</summary>
    ~Utf8String() { Dispose(false); }

    /// <inheritdoc />
    public void Dispose() => Dispose(true);

    private void Dispose(bool disposing)
    {
        if (m_disposed) return;

        if (disposing)
        {
            GC.SuppressFinalize(this);
        }

        if (Pointer != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(Pointer);
        }

        Pointer = IntPtr.Zero;
        m_disposed = true;
    }

}