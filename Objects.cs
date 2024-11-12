using System.Collections.Generic;
using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL
{
    public class Position
    {
        public string Name { get; set; }
        
        public float x { get; set; }

        public float y { get; set; }

        public float z { get; set; }
        
        public Position()
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
        }
        public Position(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Position(Vector3 value)
        {
            this.x = value.x;
            this.y = value.y;
            this.z = value.z;
        }
        
        public void Assign(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            //this.UpdateValue();
        }
        
        public void Assign(Position p)
        {
            this.x = p.x;
            this.y = p.y;
            this.z = p.z;
            //this.UpdateValue();
        }
        
        public void Assign(Vector3 p)
        {
            this.x = p.x;
            this.y = p.y;
            this.z = p.z;
            //this.UpdateValue();
        }

        /*private void UpdateValue()
        {
            if (Owner != null)
            {
                var propertyName = GetPropertyName(Owner, this);
                UpdateValueWrapper.UpdateValue(
                    Owner.ToString(),
                    propertyName,
                    new Dictionary<string, object>
                    {
                        { "x", x },
                        { "y", y },
                        { "z", z },
                    }
                );
            }
        }*/
        
        public Vector3 GetPosition()
        {
            return new Vector3(x, y, z);
        }

        public override string ToString()
        {
            return x + ", " + y + ", " + z;
        }

        public override bool Equals(object obj)
        {
            if(obj is Position)
            {
                Position p = obj as Position;
                return this.x == p.x && this.y == p.y && this.z == p.z;
            }
            else
            {
                return false;
            }
        }
    }

    public class Rotation
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        
        public Rotation()
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
        }
        
        public Rotation(Rotation r)
        {
            this.x = r.x;
            this.y = r.y;
            this.z = r.z;
        }
        
        public Rotation(Quaternion r)
        {
            this.x = r.eulerAngles.x;
            this.y = r.eulerAngles.y;
            this.z = r.eulerAngles.z;
        }
        
        public void Assign(Rotation r)
        {
            this.x = r.x;
            this.y = r.y;
            this.z = r.z;
        }
        
        public void Assign(Vector3 r)
        {
            this.x = r.x;
            this.y = r.y;
            this.z = r.z;
        }
        
        public void Assign(Quaternion r)
        {
            this.x = r.eulerAngles.x;
            this.y = r.eulerAngles.y;
            this.z = r.eulerAngles.z;
        }
    }
    
    public class Scale
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        
        public Scale()
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
        }
        
        public Scale(Scale r)
        {
            this.x = r.x;
            this.y = r.y;
            this.z = r.z;
        }
        
        public Scale(Vector3 s)
        {
            this.x = s.x;
            this.y = s.y;
            this.z = s.z;
        }
        
        public void Assign(Scale r)
        {
            this.x = r.x;
            this.y = r.y;
            this.z = r.z;
        }
        
        public void Assign(Vector3 r)
        {
            this.x = r.x;
            this.y = r.y;
            this.z = r.z;
        }
    }

    public class Path
    {
        public string Name { get; set; }

        public List<Position> Points { get; set; }

        public Path(List<Position> list)
        {
            Name = "Path";
            Points = list;
        }
        
        public Path(string name, List<Position> list)
        {
            Name = name;
            Points = list;
        }

        public override bool Equals(object obj)
        {
            if(obj is Path)
            {
                Path path = obj as Path;

                if(this.Points.Count != path.Points.Count)
                {
                    return false;
                }

                for(int i = 0; i < this.Points.Count; i++)
                {
                    if (!this.Points[i].Equals(path.Points[i]))
                        return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
