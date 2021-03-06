// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: user.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Confluent.Kafka.Examples.Protobuf {

  /// <summary>Holder for reflection information generated from user.proto</summary>
  public static partial class UserReflection {

    #region Descriptor
    /// <summary>File descriptor for user.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static UserReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cgp1c2VyLnByb3RvEiFjb25mbHVlbnQua2Fma2EuZXhhbXBsZXMucHJvdG9i",
            "dWYiQwoEVXNlchIMCgROYW1lGAEgASgJEhYKDkZhdm9yaXRlTnVtYmVyGAIg",
            "ASgFEhUKDUZhdm9yaXRlQ29sb3IYAyABKAliBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Confluent.Kafka.Examples.Protobuf.User), global::Confluent.Kafka.Examples.Protobuf.User.Parser, new[]{ "Name", "FavoriteNumber", "FavoriteColor" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class User : pb::IMessage<User> {
    private static readonly pb::MessageParser<User> _parser = new pb::MessageParser<User>(() => new User());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<User> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Confluent.Kafka.Examples.Protobuf.UserReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public User() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public User(User other) : this() {
      name_ = other.name_;
      favoriteNumber_ = other.favoriteNumber_;
      favoriteColor_ = other.favoriteColor_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public User Clone() {
      return new User(this);
    }

    /// <summary>Field number for the "Name" field.</summary>
    public const int NameFieldNumber = 1;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "FavoriteNumber" field.</summary>
    public const int FavoriteNumberFieldNumber = 2;
    private int favoriteNumber_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int FavoriteNumber {
      get { return favoriteNumber_; }
      set {
        favoriteNumber_ = value;
      }
    }

    /// <summary>Field number for the "FavoriteColor" field.</summary>
    public const int FavoriteColorFieldNumber = 3;
    private string favoriteColor_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string FavoriteColor {
      get { return favoriteColor_; }
      set {
        favoriteColor_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as User);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(User other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Name != other.Name) return false;
      if (FavoriteNumber != other.FavoriteNumber) return false;
      if (FavoriteColor != other.FavoriteColor) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (FavoriteNumber != 0) hash ^= FavoriteNumber.GetHashCode();
      if (FavoriteColor.Length != 0) hash ^= FavoriteColor.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Name.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Name);
      }
      if (FavoriteNumber != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(FavoriteNumber);
      }
      if (FavoriteColor.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(FavoriteColor);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (FavoriteNumber != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(FavoriteNumber);
      }
      if (FavoriteColor.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(FavoriteColor);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(User other) {
      if (other == null) {
        return;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.FavoriteNumber != 0) {
        FavoriteNumber = other.FavoriteNumber;
      }
      if (other.FavoriteColor.Length != 0) {
        FavoriteColor = other.FavoriteColor;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Name = input.ReadString();
            break;
          }
          case 16: {
            FavoriteNumber = input.ReadInt32();
            break;
          }
          case 26: {
            FavoriteColor = input.ReadString();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code