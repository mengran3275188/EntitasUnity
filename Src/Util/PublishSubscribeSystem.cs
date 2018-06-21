using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util
{
  public sealed class PublishSubscribeSystem
  {
    private class ReceiptInfo
    {
      public string name_;
      public Delegate delegate_;
      public ReceiptInfo() { }
      public ReceiptInfo(string n, Delegate d)
      {
        name_ = n;
        delegate_ = d;
      }
    }

    public bool RunInLogicThread
    {
      get { return run_in_logic_thread_; }
      set { run_in_logic_thread_ = value; }
    }

    public object Subscribe(string ev_name, string group, MyAction subscriber) { return AddSubscriber(ev_name, group, subscriber); }
    public object Subscribe<T0>(string ev_name, string group, MyAction<T0> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
    public object Subscribe<T0, T1>(string ev_name, string group, MyAction<T0, T1> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
    public object Subscribe<T0, T1, T2>(string ev_name, string group, MyAction<T0, T1, T2> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
    public object Subscribe<T0, T1, T2, T3>(string ev_name, string group, MyAction<T0, T1, T2, T3> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
    public object Subscribe<T0, T1, T2, T3, T4>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
    public object Subscribe<T0, T1, T2, T3, T4, T5>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
    public object Subscribe<T0, T1, T2, T3, T4, T5, T6>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5, T6> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
    public object Subscribe<T0, T1, T2, T3, T4, T5, T6, T7>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5, T6, T7> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
    public object Subscribe<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
    public object Subscribe<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
    public object Subscribe<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
    public object Subscribe<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
    public object Subscribe<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
    public object Subscribe<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> subscriber) { return AddSubscriber(ev_name, group, subscriber); }
    public object Subscribe<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string ev_name, string group, MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> subscriber) { return AddSubscriber(ev_name, group, subscriber); }

    Delegate getDelegate(string ev_name, string group)
    {
      try {
          LogUtil.Info("Publish {0} {1}", ev_name, group);

        Delegate d;
        string key = group + '#' + ev_name;
        if(subscribers_.TryGetValue(key, out d)) {
          if(null == d) {
              LogUtil.Error("Publish {0} {1}, Subscriber is null, Remove it", ev_name, group);
            subscribers_.Remove(key);
          } else {
            return d;
          }
        }
      } catch(Exception ex) {
          LogUtil.Error("PublishSubscribe.Publish({0},{1}) exception:{2}\n{3}", ev_name, group, ex.Message, ex.StackTrace);
      }
      return null;
    }

    public void Publish(string ev_name, string group)
    {
      Delegate _function = getDelegate(ev_name,group);
      MyAction function = _function as MyAction;
      if(function!=null) {
        function();
      }
    }

    public void Publish<T0>(string ev_name,string group,T0 t0)
    {
      Delegate d;
      string key = group + '#' + ev_name;
      if(subscribers_.TryGetValue(key, out d)) {
        var function = d as MyAction<T0>;
        if(function != null) {
          function(t0);
        }
      }
    }

    public void Publish<T0,T1>(string ev_name, string group, T0 t0,T1 t1)
    {
      Delegate _function = getDelegate(ev_name, group);

      var callback = _function as MyAction<T0, T1>;
      if(callback != null) {
        callback(t0, t1);
      }
    }

    public void Publish<T0, T1,T2>(string ev_name, string group, T0 t0, T1 t1,T2 t2)
    {
      Delegate _function = getDelegate(ev_name, group);
      MyAction<T0, T1,T2> function = _function as MyAction<T0, T1,T2>;
      if(function != null) {
        function(t0, t1,t2);
      }
    }

    public void Publish<T0, T1, T2,T3>(string ev_name, string group, T0 t0, T1 t1, T2 t2,T3 t3)
    {
      Delegate _function = getDelegate(ev_name, group);
      MyAction<T0, T1, T2,T3> function = _function as MyAction<T0, T1, T2,T3>;
      if(function != null) {
        function(t0, t1, t2,t3);
      }
    }


    public void Publish<T0, T1, T2, T3,T4>(string ev_name, string group, T0 t0, T1 t1, T2 t2, T3 t3,T4 t4)
    {
      Delegate _function = getDelegate(ev_name, group);
      MyAction<T0, T1, T2, T3,T4> function = _function as MyAction<T0, T1, T2, T3,T4>;
      if(function != null) {
        function(t0, t1, t2, t3,t4);
      }
    }

    public void Publish<T0, T1, T2, T3, T4,T5>(string ev_name, string group, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4,T5 t5)
    {
      Delegate _function = getDelegate(ev_name, group);
      MyAction<T0, T1, T2, T3, T4,T5> function = _function as MyAction<T0, T1, T2, T3, T4,T5>;
      if(function != null) {
        function(t0, t1, t2, t3, t4,t5);
      }
    }

    public void Publish<T0, T1, T2, T3, T4, T5,T6>(string ev_name, string group, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5,T6 t6)
    {
      Delegate _function = getDelegate(ev_name, group);
      MyAction<T0, T1, T2, T3, T4, T5,T6> function = _function as MyAction<T0, T1, T2, T3, T4, T5,T6>;
      if(function != null) {
        function(t0, t1, t2, t3, t4, t5,t6);
      }
    }

    public void Publish<T0, T1, T2, T3, T4, T5, T6,T7>(string ev_name, string group, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6,T7 t7)
    {
      Delegate _function = getDelegate(ev_name, group);
      MyAction<T0, T1, T2, T3, T4, T5, T6,T7> function = _function as MyAction<T0, T1, T2, T3, T4, T5, T6,T7>;
      if(function != null) {
        function(t0, t1, t2, t3, t4, t5, t6,t7);
      }
    }

    public void Publish<T0, T1, T2, T3, T4, T5, T6, T7,T8>(string ev_name, string group, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7,T8 t8)
    {
      Delegate _function = getDelegate(ev_name, group);
      MyAction<T0, T1, T2, T3, T4, T5, T6, T7,T8> function = _function as MyAction<T0, T1, T2, T3, T4, T5, T6, T7,T8>;
      if(function != null) {
        function(t0, t1, t2, t3, t4, t5, t6, t7,t8);
      }
    }

    public void Publish<T0, T1, T2, T3, T4, T5, T6, T7, T8,T9>(string ev_name, string group, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8,T9 t9)
    {
      Delegate _function = getDelegate(ev_name, group);
      MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8,T9> function = _function as MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8,T9>;
      if(function != null) {
        function(t0, t1, t2, t3, t4, t5, t6, t7, t8,t9);
      }
    }

    public void Publish<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9,T10>(string ev_name, string group, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9,T10 t10)
    {
      Delegate _function = getDelegate(ev_name, group);
      MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9,T10> function = _function as MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9,T10>;
      if(function != null) {
        function(t0, t1, t2, t3, t4, t5, t6, t7, t8, t9,t10);
      }
    }

    public void Publish<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11>(string ev_name, string group, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10,T11 t11)
    {
      Delegate _function = getDelegate(ev_name, group);
      MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> function = _function as MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11>;
      if(function != null) {
        function(t0, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10,t11);
      }
    }

    public void Publish<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12>(string ev_name, string group, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11,T12 t12)
    {
      Delegate _function = getDelegate(ev_name, group);
      MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> function = _function as MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12>;
      if(function != null) {
        function(t0, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11,t12);
      }
    }

    public void Publish<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12,T13>(string ev_name, string group, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12,T13 t13)
    {
      Delegate _function = getDelegate(ev_name, group);
      MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12,T13> function = _function as MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12,T13>;
      if(function != null) {
        function(t0, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12,t13);
      }
    }

    public void Publish<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,T14>(string ev_name, string group, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13,T14 t14)
    {
      Delegate _function = getDelegate(ev_name, group);
      MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,T14> function = _function as MyAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,T14>;
      if(function != null) {
        function(t0, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13,t14);
      }
    }

    public void Unsubscribe(object receipt)
    {
      ReceiptInfo r = receipt as ReceiptInfo;
      Delegate d;
      if (null != r && subscribers_.TryGetValue(r.name_, out d)) {
        subscribers_[r.name_] = Delegate.Remove(d, r.delegate_);
      }
    }

    public void Publish(string ev_name, string group, params object[] parameters)
    {
      try {
          LogUtil.Info("Publish {0} {1}", ev_name, group);

        Delegate d;
        string key = group + '#' + ev_name;
        if (subscribers_.TryGetValue(key, out d)) {
          if (null == d) {
              LogUtil.Error("Publish {0} {1}, Subscriber is null, Remove it", ev_name, group);
            subscribers_.Remove(key);
          } else {
            d.DynamicInvoke(parameters);
          }
        }
      } catch (Exception ex) {
          LogUtil.Error("PublishSubscribe.Publish({0},{1}) exception:{2}\n{3}", ev_name, group, ex.Message, ex.StackTrace);
      }
    }

    private object AddSubscriber(string ev_name, string group, Delegate d)
    {
      Delegate source;
      string key = group + '#' + ev_name;
      if(subscribers_.TryGetValue(key, out source)) {
        if(null != source)
          source = Delegate.Combine(source, d);
        else
          source = d;
      } else {
        source = d;
      }
      subscribers_ [key] = source;
      return new ReceiptInfo(key, d);
    }

    private Dictionary<string, Delegate> subscribers_ = new Dictionary<string, Delegate>();
    private bool run_in_logic_thread_ = true;

    private const string c_proxy_ev_name = "sys_proxy_publish";
    private const string c_proxy_group = "sys";
  }
}
