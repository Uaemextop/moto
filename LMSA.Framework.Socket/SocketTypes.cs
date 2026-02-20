using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace lenovo.mbg.service.framework.socket;

/// <summary>
/// Socket wrapper for device communication.
/// </summary>
public class SocketWrapper : IDisposable
{
    public bool IsConnected { get; set; }
    public IPEndPoint RemoteEndPoint { get; set; }

    public SocketWrapper() { }

    public SocketWrapper(IPEndPointInfo endPoint, bool noDelay = false, bool isHeartbeatChannel = false, int timeout = 0)
    {
    }

    public void Dispose() { }
}

/// <summary>
/// Message reader for socket communication.
/// </summary>
public class MessageReader : IDisposable
{
    public MessageReader(SocketWrapper socket, PackageSpliter spliter, RsaSocketDataSecurityFactory security) { }
    public void Dispose() { }
}

/// <summary>
/// Message writer for socket communication.
/// </summary>
public class MessageWriter : IDisposable
{
    public MessageWriter(SocketWrapper socket, bool appendSpliterString, RsaSocketDataSecurityFactory security) { }
    public void Dispose() { }
}

/// <summary>
/// Message reader/writer for socket communication protocol.
/// </summary>
public class MessageReaderAndWriter : IDisposable
{
    public SocketWrapper SocketWrapper { get; set; }

    public MessageReaderAndWriter() { }

    public MessageReaderAndWriter(MessageWriter writer, MessageReader reader) { }

    public bool Receive(string command, out long sequence, out List<PropItem> receiveData, int timeout = 15000)
    {
        sequence = 0;
        receiveData = null;
        return false;
    }

    public void Send(string command, List<string> data, long sequence) { }

    public bool SendAndReceiveSync<TSend, TReceive>(string sendCommand, string receiveCommand, TSend sendData, long sequence, out List<TReceive> receiveData)
    {
        receiveData = null;
        return false;
    }

    public void Dispose() { }
}

/// <summary>
/// File transfer wrapper for device file operations.
/// </summary>
public class FileTransferWrapper : IDisposable
{
    public SocketWrapper Channel { get; set; }

    public FileTransferWrapper() { }

    public FileTransferWrapper(IPEndPointInfo endPoint, RsaSocketDataSecurityFactory security) { }

    public void Dispose() { }
}

/// <summary>
/// Heartbeat socket wrapper.
/// </summary>
public class HeartbeatSocketWrapper : SocketWrapper
{
    public HeartbeatSocketWrapper(IPEndPointInfo endPoint, bool noDelay, RsaSocketDataSecurityFactory security) { }

    public void SetSendHeartbeatInterval(long interval) { }
    public void StartSendHeartbeat() { }
    public void StartReceiveHeartbeat() { }
    public event EventHandler<HeartbeatStoppedEventArgs> HeartbeatStopped;
}

/// <summary>
/// Package splitter for message framing.
/// </summary>
public class PackageSpliter
{
    public PackageSpliter(byte[] delimiter, bool removePackageSpliter) { }
}

/// <summary>
/// Sequence generator for messages.
/// </summary>
public class Sequence
{
    private static Sequence _instance;
    private long _current;

    public static Sequence SingleInstance => _instance ?? (_instance = new Sequence());

    public long New() => System.Threading.Interlocked.Increment(ref _current);
}

/// <summary>
/// Socket constants.
/// </summary>
public static class Constants
{
    public static Encoding Encoding => Encoding.UTF8;
}

/// <summary>
/// Property information from device.
/// </summary>
public class PropInfo
{
    public List<PropItem> Items { get; set; } = new List<PropItem>();
    public Dictionary<string, string> Props { get; set; } = new Dictionary<string, string>();

    public string GetProp(string key)
    {
        if (Props.TryGetValue(key, out var value))
            return value;
        var item = Items?.Find(p => p.Key == key);
        return item?.Value;
    }

    public int GetIntProp(string key, int defaultValue = 0)
    {
        var val = GetProp(key);
        if (int.TryParse(val, out int result))
            return result;
        return defaultValue;
    }

    public void AddOrUpdateProp(string key, string value)
    {
        Props[key] = value;
        var item = Items?.Find(p => p.Key == key);
        if (item != null)
            item.Value = value;
        else
            Items?.Add(new PropItem { Key = key, Value = value });
    }

    public void AddOrUpdateProp(List<PropItem> items)
    {
        if (items == null) return;
        foreach (var item in items)
        {
            AddOrUpdateProp(item.Key, item.Value);
        }
    }

    public void AddOrUpdateProp(List<PropItem> items, Dictionary<string, string> keyMapping)
    {
        if (items == null) return;
        foreach (var item in items)
        {
            string targetKey = item.Key;
            if (keyMapping != null && keyMapping.TryGetValue(item.Key, out var mappedKey))
                targetKey = mappedKey;
            AddOrUpdateProp(targetKey, item.Value);
        }
    }

    public void AddOrUpdateProp(Dictionary<string, string> props)
    {
        if (props == null) return;
        foreach (var kvp in props)
        {
            AddOrUpdateProp(kvp.Key, kvp.Value);
        }
    }

    public void AddOrUpdateProp(PropItem item)
    {
        if (item != null)
            AddOrUpdateProp(item.Key, item.Value);
    }

    public void Reset()
    {
        Items?.Clear();
        Props?.Clear();
    }

    public void Reset(string key, string value)
    {
        AddOrUpdateProp(key, value);
    }
}

/// <summary>
/// Individual property item.
/// </summary>
public class PropItem
{
    public string Key { get; set; }
    public string Value { get; set; }
}

/// <summary>
/// IP endpoint information for device connection.
/// </summary>
public class IPEndPointInfo
{
    public string IPAddress { get; set; }
    public int Point { get; set; }
    public string Address { get; set; }
    public int Port { get; set; }
    public IPEndPoint ToIPEndPoint() => new IPEndPoint(System.Net.IPAddress.Parse(IPAddress ?? Address ?? "127.0.0.1"), Point > 0 ? Point : Port);
}

/// <summary>
/// RSA socket data security factory.
/// </summary>
public class RsaSocketDataSecurityFactory
{
}

/// <summary>
/// Heartbeat stopped event args.
/// </summary>
public class HeartbeatStoppedEventArgs : EventArgs
{
    public string Reason { get; set; }
}

/// <summary>
/// Interface for message management.
/// </summary>
public interface IMessageManager : IDisposable
{
    bool IsConnected { get; }
    RsaSocketDataSecurityFactory RsaSocketEncryptHelper { get; }
    MessageReaderAndWriter CreateMessageReaderAndWriter(int timeout = 0);
    void StartHeartbeat(long sendHeartbeatInterval = 1000L, long receiveHeartbeatTimeout = 10000L);
    event EventHandler<HeartbeatStoppedEventArgs> HeartbeatStopped;
    SocketWrapper GetHeartbeatChannel();
}

/// <summary>
/// Interface for file transfer management.
/// </summary>
public interface IFileTransferManager : IDisposable
{
    bool IsDispose { get; set; }
    string InstanceID { get; }
    IPEndPointInfo GetIPEndPointInfo();
    FileTransferWrapper CreateFileTransfer(long messageSequence);
}

/// <summary>
/// Abstract base class for connection monitors.
/// </summary>
public abstract class AbstractConnectionMonitorEx
{
    public abstract void Start();
    public abstract void Stop();
    public abstract void Dispose();
}
