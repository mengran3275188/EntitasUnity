using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace Spatial
{
  class TestSpaceObject : ISpaceObject
  {
    public TestSpaceObject(uint id)
    {
      id_ = id;
    }

    public uint GetID()
    {
      return id_;
    }
        public int GetId()
        {
            return (int)id_;
        }

    public SpatialObjType GetObjType()
    {
      return obj_type_;
    }

    public void SetObjType(SpatialObjType ot)
    {
      obj_type_ = ot;
    }

    public Vector3 GetPosition()
    {
      return pos_;
    }

    public float GetRadius() { return 1.0f; }
    public Vector3 GetVelocity() { return new Vector3(); }
    public bool IsAvoidable() { return false; }
    
    public object RealObject
    {
      get
      {
        return this;
      }
    }
    
    public void SetPosition(Vector3 pos)
    {
      old_pos_ = pos_;
      pos_ = pos;
    }
    public void SetFaceDirection(double dict)
    {
      direction_ = dict;
      cos_dir_ = Math.Cos(direction_);
      sin_dir_ = Math.Sin(direction_);
    }

    public double GetFaceDirection()
    {
      return direction_;
    }
    public double CosDir
    {
      get { return cos_dir_; }
    }
    public double SinDir
    {
      get { return sin_dir_; }
    }

    public Vector3 Position
    {
      set { pos_ = value; }
      get { return pos_; }
    }
    // private attributes-----------------------------------------------------
    private uint id_;
    private Vector3 pos_ = new Vector3(0, 0, 0);
    private Vector3 old_pos_ = new Vector3();
    private double direction_ = 0;
    private double cos_dir_ = 1;
    private double sin_dir_ = 0;
    private List<ISpaceObject> collide_objects_ = new List<ISpaceObject>();
    private SpatialObjType obj_type_ = SpatialObjType.kUser;
  }
}
