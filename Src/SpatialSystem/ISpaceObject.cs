using System;
using System.Collections.Generic;
using Util.MyMath;

namespace Spatial
{

  public enum SpatialObjType
  {
    kTypeBegin = 0,
    kUser,
    kNPC,
    kTypeEnd
  }
  public class BlockType
  {
    public const byte OUT_OF_BLOCK = (byte)0x80;
    public const byte NOT_BLOCK = (byte)0x00;
    public const byte STATIC_BLOCK = (byte)0x01;
    public const byte DYNAMIC_BLOCK = (byte)0x02;
    public const byte RESERVED_BLOCK = (byte)0x03;
    public const byte TYPE_MASK = (byte)0x03;

    public static byte GetBlockType(byte status)
    {
      return (byte)(status & TYPE_MASK);
    }
  };
  public class CellPos
  {
    public CellPos()
    {
      this.row = 0;
      this.col = 0;
    }
    public CellPos(int row, int col)
    {
      this.row = row;
      this.col = col;
    }
    public CellPos Clone()
    {
      return new CellPos(this.row, this.col);
    }
    public int row = 0;
    public int col = 0;
  };

  public interface ISpaceObject
  {
    uint GetID();
    SpatialObjType GetObjType();
    Vector3 GetPosition();
    float GetRadius();
    Vector3 GetVelocity();
    bool IsAvoidable();
    object RealObject { get; }
  }

}
