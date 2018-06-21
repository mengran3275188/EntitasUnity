using System;
using System.Collections.Generic;
using System.Text;

namespace Util
{
    public class ObjectPool<T>
    {
        private readonly MyFunc<T> _factoryMethod;
        private readonly MyAction<T> _resetMethod;
        private readonly Stack<T> _objectPool;

        public ObjectPool(MyFunc<T> factoryMethod, MyAction<T> resetMethod = null)
        {
            this._factoryMethod = factoryMethod;
            this._resetMethod = resetMethod;
            this._objectPool = new Stack<T>();
        }
        public T Get()
        {
            if(this._objectPool.Count != 0)
            {
                return this._objectPool.Pop();
            }
            return this._factoryMethod();
        }
        public void Push(T obj)
        {
            if(this._resetMethod != null)
            {
                this._resetMethod(obj);
            }
            this._objectPool.Push(obj);
        }
    }
}
