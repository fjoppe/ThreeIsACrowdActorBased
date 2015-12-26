(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,Concurrency,Array,Seq,Arrays,UI,Next,Abbrev,Fresh,Collections,HashSetProxy,HashSet,JQueue,Slot1,Unchecked,An,AppendList1,Anims,requestAnimationFrame,Lazy,Array1,Trans,Option,View,Client,Attrs,DomUtility,Attr,AttrProxy,List,AnimatedAttrNode,DynamicAttrNode,View1,document,Doc,Elt,Seq1,Docs,String,CheckedInput,Mailbox,Operators,T,jQuery,NodeSet,DocElemNode,DomNodes,Var1,RegExp,Var,Easing,Easings,FlowBuilder,Flow1,Input,DoubleInterpolation,Key,ListModels,RefImpl1,ListModel,Model1,Model,Strings,encodeURIComponent,decodeURIComponent,Route,Routing,Router1,Trie1,Dictionary,window,Snap1,Async,Char,Submitter,Enumerator,ResizeArray,ResizeArrayProxy,MapModule,FSharpMap,RefImpl;
 Runtime.Define(Global,{
  WebSharper:{
   UI:{
    Next:{
     Abbrev:{
      Async:{
       Schedule:function(f)
       {
        return Concurrency.Start(Concurrency.Delay(function()
        {
         return Concurrency.Return(f(null));
        }),{
         $:0
        });
       },
       StartTo:function(comp,k)
       {
        return Concurrency.StartWithContinuations(comp,k,function()
        {
        },function()
        {
        },{
         $:0
        });
       }
      },
      Dict:{
       ToKeyArray:function(d)
       {
        var arr;
        arr=Array(d.count);
        Seq.iteri(function(i)
        {
         return function(kv)
         {
          return Arrays.set(arr,i,kv.K);
         };
        },d);
        return arr;
       },
       ToValueArray:function(d)
       {
        var arr;
        arr=Array(d.count);
        Seq.iteri(function(i)
        {
         return function(kv)
         {
          return Arrays.set(arr,i,kv.V);
         };
        },d);
        return arr;
       }
      },
      Fresh:{
       Id:function()
       {
        var _;
        _=Fresh.counter()+1;
        Fresh.counter=function()
        {
         return _;
        };
        return"uid"+Global.String(Fresh.counter());
       },
       Int:function()
       {
        var _;
        _=Fresh.counter()+1;
        Fresh.counter=function()
        {
         return _;
        };
        return Fresh.counter();
       },
       counter:Runtime.Field(function()
       {
        return 0;
       })
      },
      HashSet:{
       Except:function(excluded,included)
       {
        var set;
        set=HashSetProxy.New(HashSet.ToArray(included));
        set.ExceptWith(HashSet.ToArray(excluded));
        return set;
       },
       Filter:function(ok,set)
       {
        return HashSetProxy.New(Arrays.filter(ok,HashSet.ToArray(set)));
       },
       Intersect:function(a,b)
       {
        var set;
        set=HashSetProxy.New(HashSet.ToArray(a));
        set.IntersectWith(HashSet.ToArray(b));
        return set;
       },
       ToArray:function(set)
       {
        var arr;
        arr=Array(set.get_Count());
        set.CopyTo(arr);
        return arr;
       }
      },
      JQueue:{
       Add:function($x,$q)
       {
        var $0=this,$this=this;
        return $q.push($x);
       },
       Count:function(q)
       {
        return q.length;
       },
       Dequeue:function($q)
       {
        var $0=this,$this=this;
        return $q.shift();
       },
       Iter:function(f,q)
       {
        return Arrays.iter(f,JQueue.ToArray(q));
       },
       ToArray:function(q)
       {
        return q.slice();
       }
      },
      Mailbox:{
       StartProcessor:function(proc)
       {
        var mail,isActive,work;
        mail=[];
        isActive=[false];
        work=Concurrency.Delay(function()
        {
         return Concurrency.Combine(Concurrency.While(function()
         {
          return JQueue.Count(mail)>0;
         },Concurrency.Delay(function()
         {
          return Concurrency.Bind(proc(JQueue.Dequeue(mail)),function()
          {
           return Concurrency.Return(null);
          });
         })),Concurrency.Delay(function()
         {
          return Concurrency.Return(void(isActive[0]=false));
         }));
        });
        return function(msg)
        {
         JQueue.Add(msg,mail);
         if(!isActive[0])
          {
           isActive[0]=true;
           return Concurrency.Start(work,{
            $:0
           });
          }
         else
          {
           return null;
          }
        };
       }
      },
      Slot:Runtime.Class({},{
       Create:function(key,value)
       {
        return Slot1.New(key,value);
       }
      }),
      Slot1:Runtime.Class({
       Equals:function(o)
       {
        return Unchecked.Equals(this.key.call(null,this.value),this.key.call(null,o.get_Value()));
       },
       GetHashCode:function()
       {
        return Unchecked.Hash(this.key.call(null,this.value));
       },
       get_Value:function()
       {
        return this.value;
       }
      },{
       New:function(key,value)
       {
        var r;
        r=Runtime.New(this,{});
        r.key=key;
        r.value=value;
        return r;
       }
      }),
      U:function()
      {
       return;
      }
     },
     An:Runtime.Class({},{
      Append:function(_arg2,_arg1)
      {
       return Runtime.New(An,{
        $:0,
        $0:AppendList1.Append(_arg2.$0,_arg1.$0)
       });
      },
      Concat:function(xs)
      {
       return Runtime.New(An,{
        $:0,
        $0:AppendList1.Concat(Seq.map(function(_arg00_)
        {
         return Anims.List(_arg00_);
        },xs))
       });
      },
      Const:function(v)
      {
       return Anims.Const(v);
      },
      Delayed:function(inter,easing,dur,delay,x,y)
      {
       return{
        Compute:function(t)
        {
         return t<=delay?x:inter.Interpolate(easing.TransformTime.call(null,(t-delay)/dur),x,y);
        },
        Duration:dur+delay
       };
      },
      Map:function(f,anim)
      {
       var f1;
       f1=anim.Compute;
       return Anims.Def(anim.Duration,function(x)
       {
        return f(f1(x));
       });
      },
      Pack:function(anim)
      {
       return Runtime.New(An,{
        $:0,
        $0:AppendList1.Single({
         $:1,
         $0:anim
        })
       });
      },
      Play:function(anim)
      {
       return Concurrency.Delay(function()
       {
        return Concurrency.Bind(An.Run(function()
        {
        },Anims.Actions(anim)),function()
        {
         return Concurrency.Return(Anims.Finalize(anim));
        });
       });
      },
      Run:function(k,anim)
      {
       var dur;
       dur=anim.Duration;
       return Concurrency.FromContinuations(function(tupledArg)
       {
        var ok,loop;
        ok=tupledArg[0];
        loop=function(start,now)
        {
         var t;
         t=now-start;
         k(anim.Compute.call(null,t));
         return t<=dur?void requestAnimationFrame(function(t1)
         {
          return loop(start,t1);
         }):ok(null);
        };
        requestAnimationFrame(function(t)
        {
         return loop(t,t);
        });
        return;
       });
      },
      Simple:function(inter,easing,dur,x,y)
      {
       return{
        Compute:function(t)
        {
         return inter.Interpolate(easing.TransformTime.call(null,t/dur),x,y);
        },
        Duration:dur
       };
      },
      WhenDone:function(f,main)
      {
       return An.Append(Runtime.New(An,{
        $:0,
        $0:AppendList1.Single({
         $:0,
         $0:f
        })
       }),main);
      },
      get_Empty:function()
      {
       return Runtime.New(An,{
        $:0,
        $0:AppendList1.Empty()
       });
      }
     }),
     Anims:{
      Actions:function(_arg1)
      {
       return Anims.ConcatActions(Arrays.choose(function(_arg2)
       {
        return _arg2.$==1?{
         $:1,
         $0:_arg2.$0
        }:{
         $:0
        };
       },AppendList1.ToArray(_arg1.$0)));
      },
      ConcatActions:function(xs)
      {
       var xs1,matchValue,dur,xs2;
       xs1=Seq.toArray(xs);
       matchValue=Arrays.length(xs1);
       if(matchValue===0)
        {
         return Anims.Const(null);
        }
       else
        {
         if(matchValue===1)
          {
           return Arrays.get(xs1,0);
          }
         else
          {
           dur=Seq.max(Seq.map(function(anim)
           {
            return anim.Duration;
           },xs1));
           xs2=Arrays.map(function(anim)
           {
            return Anims.Prolong(dur,anim);
           },xs1);
           return Anims.Def(dur,function(t)
           {
            return Arrays.iter(function(anim)
            {
             return anim.Compute.call(null,t);
            },xs2);
           });
          }
        }
      },
      Const:function(v)
      {
       return Anims.Def(0,function()
       {
        return v;
       });
      },
      Def:function(d,f)
      {
       return{
        Compute:f,
        Duration:d
       };
      },
      Finalize:function(_arg1)
      {
       return Arrays.iter(function(_arg2)
       {
        return _arg2.$==0?_arg2.$0.call(null,null):null;
       },AppendList1.ToArray(_arg1.$0));
      },
      List:function(_arg1)
      {
       return _arg1.$0;
      },
      Prolong:function(nextDuration,anim)
      {
       var comp,dur,last;
       comp=anim.Compute;
       dur=anim.Duration;
       last=Lazy.Create(function()
       {
        return anim.Compute.call(null,anim.Duration);
       });
       return{
        Compute:function(t)
        {
         return t>=dur?last.eval():comp(t);
        },
        Duration:nextDuration
       };
      }
     },
     AppendList1:{
      Append:function(x,y)
      {
       var matchValue;
       matchValue=[x,y];
       return matchValue[0].$==0?matchValue[1]:matchValue[1].$==0?matchValue[0]:{
        $:2,
        $0:x,
        $1:y
       };
      },
      Concat:function(xs)
      {
       var a;
       a=Seq.toArray(xs);
       return Array1.MapReduce(function(x)
       {
        return x;
       },AppendList1.Empty(),function(_arg00_)
       {
        return function(_arg10_)
        {
         return AppendList1.Append(_arg00_,_arg10_);
        };
       },a);
      },
      Empty:function()
      {
       return{
        $:0
       };
      },
      FromArray:function(xs)
      {
       var matchValue;
       matchValue=xs.length;
       return matchValue===0?{
        $:0
       }:matchValue===1?{
        $:1,
        $0:Arrays.get(xs,0)
       }:{
        $:3,
        $0:xs.slice()
       };
      },
      Single:function(x)
      {
       return{
        $:1,
        $0:x
       };
      },
      ToArray:function(xs)
      {
       var out,loop;
       out=[];
       loop=function(xs1)
       {
        var y;
        if(xs1.$==1)
         {
          return JQueue.Add(xs1.$0,out);
         }
        else
         {
          if(xs1.$==2)
           {
            y=xs1.$1;
            loop(xs1.$0);
            return loop(y);
           }
          else
           {
            return xs1.$==3?Arrays.iter(function(v)
            {
             return JQueue.Add(v,out);
            },xs1.$0):null;
           }
         }
       };
       loop(xs);
       return JQueue.ToArray(out);
      }
     },
     Array:{
      MapReduce:function(f,z,re,a)
      {
       var loop;
       loop=function(off,len)
       {
        var l2,a1,b,l21,a2,b1;
        if(len<=0)
         {
          return z;
         }
        else
         {
          if(len===1)
           {
            if(off>=0?off<Arrays.length(a):false)
             {
              return f(Arrays.get(a,off));
             }
            else
             {
              l2=len/2>>0;
              a1=loop(off,l2);
              b=loop(off+l2,len-l2);
              return(re(a1))(b);
             }
           }
          else
           {
            l21=len/2>>0;
            a2=loop(off,l21);
            b1=loop(off+l21,len-l21);
            return(re(a2))(b1);
           }
         }
       };
       return loop(0,Arrays.length(a));
      }
     },
     Client:{
      AnimatedAttrNode:Runtime.Class({
       GetChangeAnim:function(parent)
       {
        var matchValue,a=this;
        matchValue=[this.visible,this.logical];
        return An.WhenDone(function()
        {
         return a.sync(parent);
        },matchValue[0].$==1?matchValue[1].$==1?a.dirty?An.Pack(An.Map(function(v)
        {
         return a.pushVisible(parent,v);
        },Trans.AnimateChange(a.tr,matchValue[0].$0,matchValue[1].$0))):An.get_Empty():An.get_Empty():An.get_Empty());
       },
       GetEnterAnim:function(parent)
       {
        var matchValue,a=this;
        matchValue=[this.visible,this.logical];
        return An.WhenDone(function()
        {
         return a.sync(parent);
        },matchValue[0].$==1?matchValue[1].$==1?a.dirty?An.Pack(An.Map(function(v)
        {
         return a.pushVisible(parent,v);
        },Trans.AnimateChange(a.tr,matchValue[0].$0,matchValue[1].$0))):matchValue[0].$==0?matchValue[1].$==1?An.Pack(An.Map(function(v)
        {
         return a.pushVisible(parent,v);
        },Trans.AnimateEnter(a.tr,matchValue[1].$0))):An.get_Empty():An.get_Empty():matchValue[0].$==0?matchValue[1].$==1?An.Pack(An.Map(function(v)
        {
         return a.pushVisible(parent,v);
        },Trans.AnimateEnter(a.tr,matchValue[1].$0))):An.get_Empty():An.get_Empty():matchValue[0].$==0?matchValue[1].$==1?An.Pack(An.Map(function(v)
        {
         return a.pushVisible(parent,v);
        },Trans.AnimateEnter(a.tr,matchValue[1].$0))):An.get_Empty():An.get_Empty());
       },
       GetExitAnim:function(parent)
       {
        var matchValue,a=this;
        matchValue=this.visible;
        return An.WhenDone(function()
        {
         a.dirty=true;
         a.visible={
          $:0
         };
         return;
        },matchValue.$==1?An.Pack(An.Map(function(v)
        {
         return a.pushVisible(parent,v);
        },Trans.AnimateExit(a.tr,matchValue.$0))):An.get_Empty());
       },
       Init:function()
       {
        return null;
       },
       Sync:function()
       {
        return null;
       },
       get_Changed:function()
       {
        return this.updates;
       },
       pushVisible:function(el,v)
       {
        this.visible={
         $:1,
         $0:v
        };
        this.dirty=true;
        return(this.push.call(null,el))(v);
       },
       sync:function(p)
       {
        if(this.dirty)
         {
          Option.iter(this.push.call(null,p),this.logical);
          this.visible=this.logical;
          this.dirty=false;
          return;
         }
        else
         {
          return null;
         }
       }
      },{
       New:function(tr,view,push)
       {
        var r;
        r=Runtime.New(this,{});
        r.tr=tr;
        r.push=push;
        r.logical={
         $:0
        };
        r.visible={
         $:0
        };
        r.dirty=true;
        r.updates=View.Map(function(x)
        {
         r.logical={
          $:1,
          $0:x
         };
         r.dirty=true;
         return;
        },view);
        return r;
       }
      }),
      Attr:{
       Animated:function(name,tr,view,attr)
       {
        return Attrs.Animated(tr,view,function(el)
        {
         return function(v)
         {
          return DomUtility.SetAttr(el,name,attr(v));
         };
        });
       },
       AnimatedStyle:function(name,tr,view,attr)
       {
        return Attrs.Animated(tr,view,function(el)
        {
         return function(v)
         {
          return DomUtility.SetStyle(el,name,attr(v));
         };
        });
       },
       Class:function(name)
       {
        return Attrs.Static(function(el)
        {
         return DomUtility.AddClass(el,name);
        });
       },
       ContentEditableHtml:function(_var)
       {
        var arg10;
        arg10=Attr.CustomVar(_var,function(e)
        {
         return function(v)
         {
          e.innerHTML=v;
         };
        },function(e)
        {
         return{
          $:1,
          $0:e.innerHTML
         };
        });
        return AttrProxy.Append(AttrProxy.Create("contenteditable","true"),arg10);
       },
       ContentEditableText:function(_var)
       {
        var arg10;
        arg10=Attr.CustomVar(_var,function(e)
        {
         return function(v)
         {
          e.textContent=v;
         };
        },function(e)
        {
         return{
          $:1,
          $0:e.textContent
         };
        });
        return AttrProxy.Append(AttrProxy.Create("contenteditable","true"),arg10);
       },
       CustomValue:function(_var,toString,fromString)
       {
        return Attr.CustomVar(_var,function(e)
        {
         return function(v)
         {
          e.value=toString(v);
         };
        },function(e)
        {
         return fromString(e.value);
        });
       },
       CustomVar:function(_var,set,get)
       {
        var onChange,set1;
        onChange=function(el)
        {
         return function()
         {
          return _var.UpdateMaybe(function(v)
          {
           var matchValue;
           matchValue=get(el);
           return matchValue.$==1?!Unchecked.Equals(matchValue.$0,v)?matchValue:{
            $:0
           }:{
            $:0
           };
          });
         };
        };
        set1=function(e)
        {
         return function(v)
         {
          var matchValue;
          matchValue=get(e);
          return matchValue.$==1?Unchecked.Equals(matchValue.$0,v)?null:(set(e))(v):(set(e))(v);
         };
        };
        return AttrProxy.Concat(List.ofArray([Attr.Handler("change",onChange),Attr.Handler("input",onChange),Attr.Handler("keyup",onChange),Attr.DynamicCustom(set1,_var.get_View())]));
       },
       Dynamic:function(name,view)
       {
        return Attrs.Dynamic(view,function()
        {
        },function(el)
        {
         return function(v)
         {
          return DomUtility.SetAttr(el,name,v);
         };
        });
       },
       DynamicClass:function(name,view,ok)
       {
        return Attrs.Dynamic(view,function()
        {
        },function(el)
        {
         return function(v)
         {
          return ok(v)?DomUtility.AddClass(el,name):DomUtility.RemoveClass(el,name);
         };
        });
       },
       DynamicCustom:function(set,view)
       {
        return Attrs.Dynamic(view,function()
        {
        },set);
       },
       DynamicPred:function(name,predView,valView)
       {
        var viewFn;
        viewFn=function(el)
        {
         return function(tupledArg)
         {
          var v;
          v=tupledArg[1];
          return tupledArg[0]?DomUtility.SetAttr(el,name,v):DomUtility.RemoveAttr(el,name);
         };
        };
        return Attrs.Dynamic(View.Map2(function(pred)
        {
         return function(value)
         {
          return[pred,value];
         };
        },predView,valView),function()
        {
        },viewFn);
       },
       DynamicProp:function(name,view)
       {
        return Attrs.Dynamic(view,function()
        {
        },function(el)
        {
         return function(v)
         {
          el[name]=v;
         };
        });
       },
       DynamicStyle:function(name,view)
       {
        return Attrs.Dynamic(view,function()
        {
        },function(el)
        {
         return function(v)
         {
          return DomUtility.SetStyle(el,name,v);
         };
        });
       },
       Handler:function(name,callback)
       {
        return Attrs.Static(function(el)
        {
         return el.addEventListener(name,callback(el),false);
        });
       },
       HandlerView:function(name,view,callback)
       {
        var id;
        id=Fresh.Id();
        return Attrs.Dynamic(view,function(el)
        {
         var callback1;
         callback1=callback(el);
         return el.addEventListener(name,function(ev)
         {
          return(callback1(ev))(el[id]);
         },false);
        },function(el)
        {
         return function(x)
         {
          el[id]=x;
         };
        });
       },
       OnAfterRender:function(callback)
       {
        return Attrs.Mk(0,{
         $:4,
         $0:callback
        });
       },
       Style:function(name,value)
       {
        return Attrs.Static(function(el)
        {
         return DomUtility.SetStyle(el,name,value);
        });
       },
       ValidateForm:Runtime.Field(function()
       {
        return Attr.OnAfterRender(function(e)
        {
         return Global.H5F?Global.H5F.setup(e):undefined;
        });
       }),
       Value:function(_var)
       {
        return Attr.CustomValue(_var,function(x)
        {
         return x;
        },function(x)
        {
         return{
          $:1,
          $0:x
         };
        });
       }
      },
      AttrProxy:Runtime.Class({},{
       Append:function(a,b)
       {
        return Attrs.Mk(a.Flags|b.Flags,Attrs.AppendTree(a.Tree,b.Tree));
       },
       Concat:function(xs)
       {
        var a;
        a=Seq.toArray(xs);
        return Array1.MapReduce(function(x)
        {
         return x;
        },AttrProxy.get_Empty(),function(arg00)
        {
         return function(arg10)
         {
          return AttrProxy.Append(arg00,arg10);
         };
        },a);
       },
       Create:function(name,value)
       {
        return Attrs.Static(function(el)
        {
         return DomUtility.SetAttr(el,name,value);
        });
       },
       Handler:function(event,q)
       {
        return Attrs.Static(function(el)
        {
         return el.addEventListener(event,q(el),false);
        });
       },
       get_Empty:function()
       {
        return Attrs.EmptyAttr();
       }
      }),
      Attrs:{
       Animated:function(tr,view,set)
       {
        var node,flags;
        node=AnimatedAttrNode.New(tr,view,set);
        flags=4;
        if(Trans.CanAnimateEnter(tr))
         {
          flags=flags|1;
         }
        if(Trans.CanAnimateExit(tr))
         {
          flags=flags|2;
         }
        return Attrs.Mk(flags,{
         $:1,
         $0:node
        });
       },
       AppendTree:function(a,b)
       {
        var matchValue;
        matchValue=[a,b];
        return matchValue[0].$==0?matchValue[1]:matchValue[1].$==0?matchValue[0]:{
         $:2,
         $0:a,
         $1:b
        };
       },
       Dynamic:function(view,init,set)
       {
        return Attrs.Mk(0,{
         $:1,
         $0:DynamicAttrNode.New(view,init,set)
        });
       },
       EmptyAttr:Runtime.Field(function()
       {
        return Attrs.Mk(0,{
         $:0
        });
       }),
       GetAnim:function(dyn,f)
       {
        return An.Concat(Arrays.map(function(n)
        {
         return(f(n))(dyn.DynElem);
        },dyn.DynNodes));
       },
       GetChangeAnim:function(dyn)
       {
        return Attrs.GetAnim(dyn,function(n)
        {
         return function(arg00)
         {
          return n.GetChangeAnim(arg00);
         };
        });
       },
       GetEnterAnim:function(dyn)
       {
        return Attrs.GetAnim(dyn,function(n)
        {
         return function(arg00)
         {
          return n.GetEnterAnim(arg00);
         };
        });
       },
       GetExitAnim:function(dyn)
       {
        return Attrs.GetAnim(dyn,function(n)
        {
         return function(arg00)
         {
          return n.GetExitAnim(arg00);
         };
        });
       },
       HasChangeAnim:function(attr)
       {
        return(attr.DynFlags&4)!==0;
       },
       HasEnterAnim:function(attr)
       {
        return(attr.DynFlags&1)!==0;
       },
       HasExitAnim:function(attr)
       {
        return(attr.DynFlags&2)!==0;
       },
       Insert:function(elem,tree)
       {
        var nodes,oar,loop;
        nodes=[];
        oar=[];
        loop=function(node)
        {
         var n,b;
         if(node.$==1)
          {
           n=node.$0;
           n.Init(elem);
           return JQueue.Add(n,nodes);
          }
         else
          {
           if(node.$==2)
            {
             b=node.$1;
             loop(node.$0);
             return loop(b);
            }
           else
            {
             return node.$==3?node.$0.call(null,elem):node.$==4?JQueue.Add(node.$0,oar):null;
            }
          }
        };
        loop(tree.Tree);
        return Runtime.DeleteEmptyFields({
         DynElem:elem,
         DynFlags:tree.Flags,
         DynNodes:JQueue.ToArray(nodes),
         OnAfterRender:(JQueue.Count(oar)===0?{
          $:0
         }:{
          $:1,
          $0:function(el)
          {
           return JQueue.Iter(function(f)
           {
            return f(el);
           },oar);
          }
         }).$0
        },["OnAfterRender"]);
       },
       Mk:function(flags,tree)
       {
        return Runtime.New(AttrProxy,{
         Flags:flags,
         Tree:tree
        });
       },
       Static:function(attr)
       {
        return Attrs.Mk(0,{
         $:3,
         $0:attr
        });
       },
       Sync:function(elem,dyn)
       {
        return Arrays.iter(function(d)
        {
         return d.Sync(elem);
        },dyn.DynNodes);
       },
       Updates:function(dyn)
       {
        var p,a;
        p=function(x)
        {
         return function(y)
         {
          return View.Map2(function()
          {
           return function()
           {
            return null;
           };
          },x,y);
         };
        };
        a=dyn.DynNodes;
        return Array1.MapReduce(function(x)
        {
         return x.get_Changed();
        },View1.Const(null),p,a);
       }
      },
      CheckedInput:Runtime.Class({
       get_Input:function()
       {
        return this.$==1?this.$0:this.$==2?this.$0:this.$1;
       }
      }),
      Doc:Runtime.Class({
       ReplaceInDom:function(elt)
       {
        var rdelim;
        rdelim=document.createTextNode("");
        elt.parentNode.replaceChild(rdelim,elt);
        return Doc.RunBefore(rdelim,this);
       },
       get_DocNode:function()
       {
        return this.docNode;
       },
       get_Updates:function()
       {
        return this.updates;
       }
      },{
       Append:function(a,b)
       {
        var x;
        x=View.Map2(function()
        {
         return function()
         {
          return null;
         };
        },a.get_Updates(),b.get_Updates());
        return Doc.Mk({
         $:0,
         $0:a.get_DocNode(),
         $1:b.get_DocNode()
        },x);
       },
       BindView:function(f,view)
       {
        return Doc.EmbedView(View.Map(f,view));
       },
       Button:function(caption,attrs,action)
       {
        var attrs1;
        attrs1=AttrProxy.Concat(attrs);
        return Elt.New1(Doc.Clickable("button",action),attrs1,Doc.TextNode(caption));
       },
       ButtonView:function(caption,attrs,view,action)
       {
        var attrs1;
        attrs1=AttrProxy.Concat(Seq.append([Attr.HandlerView("click",view,function()
        {
         return function()
         {
          return action;
         };
        })],attrs));
        return Elt.New1(DomUtility.CreateElement("button"),attrs1,Doc.TextNode(caption));
       },
       CheckBox:function(attrs,chk)
       {
        var el;
        el=DomUtility.CreateElement("input");
        el.addEventListener("click",function()
        {
         return chk.Set(el.checked);
        },false);
        return Elt.New1(el,AttrProxy.Concat(Seq.toList(Seq.delay(function()
        {
         return Seq.append(attrs,Seq.delay(function()
         {
          return Seq.append([AttrProxy.Create("type","checkbox")],Seq.delay(function()
          {
           return[Attr.DynamicProp("checked",chk.get_View())];
          }));
         }));
        }))),Doc.get_Empty());
       },
       CheckBoxGroup:function(attrs,item,chk)
       {
        var rvi,predicate,checkedView,attrs1,el;
        rvi=chk.get_View();
        predicate=function(x)
        {
         return Unchecked.Equals(x,item);
        };
        checkedView=View.Map(function(list)
        {
         return Seq.exists(predicate,list);
        },rvi);
        attrs1=AttrProxy.Concat(List.append(List.ofArray([AttrProxy.Create("type","checkbox"),AttrProxy.Create("name",chk.get_Id()),AttrProxy.Create("value",Fresh.Id()),Attr.DynamicProp("checked",checkedView)]),List.ofSeq(attrs)));
        el=DomUtility.CreateElement("input");
        el.addEventListener("click",function()
        {
         var chkd;
         chkd=el.checked;
         return chk.Update(function(obs)
         {
          return Seq.toList(Seq1.distinct(chkd?List.append(obs,List.ofArray([item])):List.filter(function(x1)
          {
           return!Unchecked.Equals(x1,item);
          },obs)));
         });
        },false);
        return Elt.New1(el,attrs1,Doc.get_Empty());
       },
       Clickable:function(elem,action)
       {
        var el;
        el=DomUtility.CreateElement(elem);
        el.addEventListener("click",function(ev)
        {
         ev.preventDefault();
         return action(null);
        },false);
        return el;
       },
       Concat:function(xs)
       {
        var a;
        a=Seq.toArray(xs);
        return Array1.MapReduce(function(x)
        {
         return x;
        },Doc.get_Empty(),function(arg00)
        {
         return function(arg10)
         {
          return Doc.Append(arg00,arg10);
         };
        },a);
       },
       Convert:function(render,view)
       {
        return Doc.Flatten(View.Convert(render,view));
       },
       ConvertBy:function(key,render,view)
       {
        return Doc.Flatten(View.ConvertBy(key,render,view));
       },
       ConvertSeq:function(render,view)
       {
        return Doc.Flatten(View.ConvertSeq(render,view));
       },
       ConvertSeqBy:function(key,render,view)
       {
        return Doc.Flatten(View.ConvertSeqBy(key,render,view));
       },
       Element:function(name,attr,children)
       {
        var attr1,arg20;
        attr1=AttrProxy.Concat(attr);
        arg20=Doc.Concat(children);
        return Elt.New1(DomUtility.CreateElement(name),attr1,arg20);
       },
       EmbedView:function(view)
       {
        var node,x;
        node=Docs.CreateEmbedNode();
        x=View.Map(function()
        {
        },View1.Bind(function(doc)
        {
         Docs.UpdateEmbedNode(node,doc.get_DocNode());
         return doc.get_Updates();
        },view));
        return Doc.Mk({
         $:2,
         $0:node
        },x);
       },
       Flatten:function(view)
       {
        return Doc.EmbedView(View.Map(function(arg00)
        {
         return Doc.Concat(arg00);
        },view));
       },
       FloatInput:function(attr,_var)
       {
        return Doc.InputInternal("input",function(el)
        {
         return Seq.append(attr,[Attr.CustomValue(_var,function(i)
         {
          return i.get_Input();
         },function(s)
         {
          var _,i;
          if(String.isBlank(s))
           {
            _=(el.checkValidity?el.checkValidity():true)?Runtime.New(CheckedInput,{
             $:2,
             $0:s
            }):Runtime.New(CheckedInput,{
             $:1,
             $0:s
            });
           }
          else
           {
            i=+s;
            _=Global.isNaN(i)?Runtime.New(CheckedInput,{
             $:1,
             $0:s
            }):Runtime.New(CheckedInput,{
             $:0,
             $0:i,
             $1:s
            });
           }
          return{
           $:1,
           $0:_
          };
         }),AttrProxy.Create("type","number")]);
        });
       },
       FloatInputUnchecked:function(attr,_var)
       {
        var parseFloat;
        parseFloat=function(s)
        {
         var pd;
         if(String.isBlank(s))
          {
           return{
            $:1,
            $0:0
           };
          }
         else
          {
           pd=+s;
           return Global.isNaN(pd)?{
            $:0
           }:{
            $:1,
            $0:pd
           };
          }
        };
        return Doc.InputInternal("input",function()
        {
         return Seq.append(attr,[_var.Get()===0?AttrProxy.Create("value","0"):AttrProxy.get_Empty(),Attr.CustomValue(_var,function(value)
         {
          return Global.String(value);
         },parseFloat),AttrProxy.Create("type","number")]);
        });
       },
       Input:function(attr,_var)
       {
        return Doc.InputInternal("input",function()
        {
         return Seq.append(attr,[Attr.Value(_var)]);
        });
       },
       InputArea:function(attr,_var)
       {
        return Doc.InputInternal("textarea",function()
        {
         return Seq.append(attr,[Attr.Value(_var)]);
        });
       },
       InputInternal:function(elemTy,attr)
       {
        var el;
        el=DomUtility.CreateElement(elemTy);
        return Elt.New1(el,AttrProxy.Concat(attr(el)),Doc.get_Empty());
       },
       IntInput:function(attr,_var)
       {
        return Doc.InputInternal("input",function(el)
        {
         return Seq.append(attr,[Attr.CustomValue(_var,function(i)
         {
          return i.get_Input();
         },function(s)
         {
          var _,i;
          if(String.isBlank(s))
           {
            _=(el.checkValidity?el.checkValidity():true)?Runtime.New(CheckedInput,{
             $:2,
             $0:s
            }):Runtime.New(CheckedInput,{
             $:1,
             $0:s
            });
           }
          else
           {
            i=+s;
            _=Global.isNaN(i)?Runtime.New(CheckedInput,{
             $:1,
             $0:s
            }):Runtime.New(CheckedInput,{
             $:0,
             $0:i,
             $1:s
            });
           }
          return{
           $:1,
           $0:_
          };
         }),AttrProxy.Create("type","number"),AttrProxy.Create("step","1")]);
        });
       },
       IntInputUnchecked:function(attr,_var)
       {
        var parseInt;
        parseInt=function(s)
        {
         var pd;
         if(String.isBlank(s))
          {
           return{
            $:1,
            $0:0
           };
          }
         else
          {
           pd=+s;
           return pd!==pd>>0?{
            $:0
           }:{
            $:1,
            $0:pd
           };
          }
        };
        return Doc.InputInternal("input",function()
        {
         return Seq.append(attr,[_var.Get()===0?AttrProxy.Create("value","0"):AttrProxy.get_Empty(),Attr.CustomValue(_var,function(value)
         {
          return Global.String(value);
         },parseInt),AttrProxy.Create("type","number"),AttrProxy.Create("step","1")]);
        });
       },
       Link:function(caption,attrs,action)
       {
        var arg10,attrs1;
        arg10=AttrProxy.Concat(attrs);
        attrs1=AttrProxy.Append(AttrProxy.Create("href","#"),arg10);
        return Elt.New1(Doc.Clickable("a",action),attrs1,Doc.TextNode(caption));
       },
       LinkView:function(caption,attrs,view,action)
       {
        var attrs1;
        attrs1=AttrProxy.Concat(Seq.append([Attr.HandlerView("click",view,function()
        {
         return function()
         {
          return action;
         };
        }),AttrProxy.Create("href","#")],attrs));
        return Elt.New1(DomUtility.CreateElement("a"),attrs1,Doc.TextNode(caption));
       },
       Mk:function(node,updates)
       {
        return Doc.New(node,updates);
       },
       New:function(docNode,updates)
       {
        var r;
        r=Runtime.New(this,{});
        r.docNode=docNode;
        r.updates=updates;
        return r;
       },
       PasswordBox:function(attr,_var)
       {
        return Doc.InputInternal("input",function()
        {
         return Seq.append(attr,[Attr.Value(_var),AttrProxy.Create("type","password")]);
        });
       },
       Radio:function(attrs,value,_var)
       {
        var el,valAttr,op_EqualsEqualsGreater;
        el=DomUtility.CreateElement("input");
        el.addEventListener("click",function()
        {
         return _var.Set(value);
        },false);
        valAttr=Attr.DynamicProp("checked",View.Map(function(x)
        {
         return Unchecked.Equals(x,value);
        },_var.get_View()));
        op_EqualsEqualsGreater=function(k,v)
        {
         return AttrProxy.Create(k,v);
        };
        return Elt.New1(el,AttrProxy.Concat(List.append(List.ofArray([op_EqualsEqualsGreater("type","radio"),op_EqualsEqualsGreater("name",_var.get_Id()),valAttr]),List.ofSeq(attrs))),Doc.get_Empty());
       },
       Run:function(parent,doc)
       {
        var d,st;
        d=doc.get_DocNode();
        Docs.LinkElement(parent,d);
        st=Docs.CreateRunState(parent,d);
        return View1.Sink(Mailbox.StartProcessor(function()
        {
         return Docs.PerformAnimatedUpdate(st,d);
        }),doc.get_Updates());
       },
       RunAfter:function(ldelim,doc)
       {
        var rdelim;
        rdelim=document.createTextNode("");
        ldelim.parentNode.insertBefore(rdelim,ldelim.nextSibling);
        return Doc.RunBetween(ldelim,rdelim,doc);
       },
       RunAfterById:function(id,doc)
       {
        var matchValue;
        matchValue=DomUtility.Doc().getElementById(id);
        return Unchecked.Equals(matchValue,null)?Operators.FailWith("invalid id: "+id):Doc.RunAfter(matchValue,doc);
       },
       RunAppend:function(parent,doc)
       {
        var rdelim;
        rdelim=document.createTextNode("");
        parent.appendChild(rdelim);
        return Doc.RunBefore(rdelim,doc);
       },
       RunAppendById:function(id,doc)
       {
        var matchValue;
        matchValue=DomUtility.Doc().getElementById(id);
        return Unchecked.Equals(matchValue,null)?Operators.FailWith("invalid id: "+id):Doc.RunAppend(matchValue,doc);
       },
       RunBefore:function(rdelim,doc)
       {
        var ldelim;
        ldelim=document.createTextNode("");
        rdelim.parentNode.insertBefore(ldelim,rdelim);
        return Doc.RunBetween(ldelim,rdelim,doc);
       },
       RunBeforeById:function(id,doc)
       {
        var matchValue;
        matchValue=DomUtility.Doc().getElementById(id);
        return Unchecked.Equals(matchValue,null)?Operators.FailWith("invalid id: "+id):Doc.RunBefore(matchValue,doc);
       },
       RunBetween:function(ldelim,rdelim,doc)
       {
        var st;
        Docs.LinkPrevElement(rdelim,doc.get_DocNode());
        st=Docs.CreateDelimitedRunState(ldelim,rdelim,doc.get_DocNode());
        return View1.Sink(Mailbox.StartProcessor(function()
        {
         return Docs.PerformAnimatedUpdate(st,doc.get_DocNode());
        }),doc.get_Updates());
       },
       RunById:function(id,tr)
       {
        var matchValue;
        matchValue=DomUtility.Doc().getElementById(id);
        return Unchecked.Equals(matchValue,null)?Operators.FailWith("invalid id: "+id):Doc.Run(matchValue,tr);
       },
       RunPrepend:function(parent,doc)
       {
        var rdelim;
        rdelim=document.createTextNode("");
        parent.insertBefore(rdelim,parent.firstChild);
        return Doc.RunBefore(rdelim,doc);
       },
       RunPrependById:function(id,doc)
       {
        var matchValue;
        matchValue=DomUtility.Doc().getElementById(id);
        return Unchecked.Equals(matchValue,null)?Operators.FailWith("invalid id: "+id):Doc.RunPrepend(matchValue,doc);
       },
       Select:function(attrs,show,options,current)
       {
        return Doc.SelectDyn(attrs,show,View1.Const(options),current);
       },
       SelectDyn:function(attrs,show,vOptions,current)
       {
        var options,setSelectedItem,el1,x,selectedItemAttr,optionElements;
        options=[Runtime.New(T,{
         $:0
        })];
        setSelectedItem=function(el)
        {
         return function(item)
         {
          el.selectedIndex=Seq.findIndex(function(y)
          {
           return Unchecked.Equals(item,y);
          },options[0]);
         };
        };
        el1=DomUtility.CreateElement("select");
        x=current.get_View();
        selectedItemAttr=Attr.DynamicCustom(setSelectedItem,x);
        el1.addEventListener("change",function()
        {
         return current.UpdateMaybe(function(x2)
         {
          var y;
          y=options[0].get_Item(el1.selectedIndex);
          return Unchecked.Equals(x2,y)?{
           $:0
          }:{
           $:1,
           $0:y
          };
         });
        },false);
        optionElements=Doc.Convert(function(tupledArg)
        {
         var i,t;
         i=tupledArg[0];
         t=Doc.TextNode(show(tupledArg[1]));
         return Doc.Element("option",List.ofArray([AttrProxy.Create("value",Global.String(i))]),List.ofArray([t]));
        },View.Map(function(l)
        {
         options[0]=l;
         return Seq.mapi(function(i)
         {
          return function(x1)
          {
           return[i,x1];
          };
         },l);
        },vOptions));
        return Elt.New1(el1,AttrProxy.Append(selectedItemAttr,AttrProxy.Concat(attrs)),optionElements);
       },
       SelectDynOptional:function(attrs,noneText,show,vOptions,current)
       {
        return Doc.SelectDyn(attrs,function(_arg9)
        {
         return _arg9.$==1?show(_arg9.$0):noneText;
        },View.Map(function(options)
        {
         return Runtime.New(T,{
          $:1,
          $0:{
           $:0
          },
          $1:List.map(function(arg0)
          {
           return{
            $:1,
            $0:arg0
           };
          },options)
         });
        },vOptions),current);
       },
       SelectOptional:function(attrs,noneText,show,options,current)
       {
        return Doc.Select(attrs,function(_arg8)
        {
         return _arg8.$==1?show(_arg8.$0):noneText;
        },Runtime.New(T,{
         $:1,
         $0:{
          $:0
         },
         $1:List.map(function(arg0)
         {
          return{
           $:1,
           $0:arg0
          };
         },options)
        }),current);
       },
       Static:function(el)
       {
        return Elt.New1(el,AttrProxy.get_Empty(),Doc.get_Empty());
       },
       SvgElement:function(name,attr,children)
       {
        var attr1,arg20;
        attr1=AttrProxy.Concat(attr);
        arg20=Doc.Concat(children);
        return Elt.New1(DomUtility.CreateSvgElement(name),attr1,arg20);
       },
       TextNode:function(v)
       {
        return Doc.Mk({
         $:5,
         $0:DomUtility.CreateText(v)
        },View1.Const(null));
       },
       TextView:function(txt)
       {
        var node,x;
        node=Docs.CreateTextNode();
        x=View.Map(function(t)
        {
         return Docs.UpdateTextNode(node,t);
        },txt);
        return Doc.Mk({
         $:4,
         $0:node
        },x);
       },
       Verbatim:function(html)
       {
        var matchValue,a,append;
        matchValue=jQuery.parseHTML(html);
        a=Unchecked.Equals(matchValue,null)?[]:matchValue;
        append=function(x)
        {
         return function(y)
         {
          return{
           $:0,
           $0:x,
           $1:y
          };
         };
        };
        return Doc.Mk(Array1.MapReduce(function(e)
        {
         return{
          $:1,
          $0:Docs.CreateElemNode(e,AttrProxy.get_Empty(),{
           $:3
          })
         };
        },{
         $:3
        },append,a),View1.Const(null));
       },
       get_Empty:function()
       {
        return Doc.Mk({
         $:3
        },View1.Const(null));
       }
      }),
      DocElemNode:Runtime.Class({
       Equals:function(o)
       {
        return this.ElKey===o.ElKey;
       },
       GetHashCode:function()
       {
        return this.ElKey;
       }
      }),
      Docs:{
       ComputeChangeAnim:function(st,cur)
       {
        var arg00,relevant;
        arg00=function(n)
        {
         return Attrs.HasChangeAnim(n.Attr);
        };
        relevant=function(arg10)
        {
         return NodeSet.Filter(arg00,arg10);
        };
        return An.Concat(Arrays.map(function(n)
        {
         return Attrs.GetChangeAnim(n.Attr);
        },NodeSet.ToArray(NodeSet.Intersect(relevant(st.PreviousNodes),relevant(cur)))));
       },
       ComputeEnterAnim:function(st,cur)
       {
        return An.Concat(Arrays.map(function(n)
        {
         return Attrs.GetEnterAnim(n.Attr);
        },NodeSet.ToArray(NodeSet.Except(st.PreviousNodes,NodeSet.Filter(function(n)
        {
         return Attrs.HasEnterAnim(n.Attr);
        },cur)))));
       },
       ComputeExitAnim:function(st,cur)
       {
        return An.Concat(Arrays.map(function(n)
        {
         return Attrs.GetExitAnim(n.Attr);
        },NodeSet.ToArray(NodeSet.Except(cur,NodeSet.Filter(function(n)
        {
         return Attrs.HasExitAnim(n.Attr);
        },st.PreviousNodes)))));
       },
       CreateDelimitedElemNode:function(ldelim,rdelim,attr,children)
       {
        var el,attr1;
        el=ldelim.parentNode;
        Docs.LinkPrevElement(rdelim,children);
        attr1=Attrs.Insert(el,attr);
        return Runtime.New(DocElemNode,Runtime.DeleteEmptyFields({
         Attr:attr1,
         Children:children,
         Delimiters:[ldelim,rdelim],
         El:el,
         ElKey:Fresh.Int(),
         Render:Runtime.GetOptional(attr1.OnAfterRender).$0
        },["Render"]));
       },
       CreateDelimitedRunState:function(ldelim,rdelim,doc)
       {
        return{
         PreviousNodes:NodeSet.get_Empty(),
         Top:Docs.CreateDelimitedElemNode(ldelim,rdelim,AttrProxy.get_Empty(),doc)
        };
       },
       CreateElemNode:function(el,attr,children)
       {
        var attr1;
        Docs.LinkElement(el,children);
        attr1=Attrs.Insert(el,attr);
        return Runtime.New(DocElemNode,Runtime.DeleteEmptyFields({
         Attr:attr1,
         Children:children,
         El:el,
         ElKey:Fresh.Int(),
         Render:Runtime.GetOptional(attr1.OnAfterRender).$0
        },["Render"]));
       },
       CreateEmbedNode:function()
       {
        return{
         Current:{
          $:3
         },
         Dirty:false
        };
       },
       CreateRunState:function(parent,doc)
       {
        return{
         PreviousNodes:NodeSet.get_Empty(),
         Top:Docs.CreateElemNode(parent,AttrProxy.get_Empty(),doc)
        };
       },
       CreateTextNode:function()
       {
        return{
         Text:DomUtility.CreateText(""),
         Dirty:false,
         Value:""
        };
       },
       DoSyncElement:function(el)
       {
        var parent,ins,parent1,matchValue;
        parent=el.El;
        ins=function(doc,pos)
        {
         var d;
         if(doc.$==1)
          {
           return{
            $:1,
            $0:doc.$0.El
           };
          }
         else
          {
           if(doc.$==2)
            {
             d=doc.$0;
             if(d.Dirty)
              {
               d.Dirty=false;
               return Docs.InsertDoc(parent,d.Current,pos);
              }
             else
              {
               return ins(d.Current,pos);
              }
            }
           else
            {
             return doc.$==3?pos:doc.$==4?{
              $:1,
              $0:doc.$0.Text
             }:doc.$==5?{
              $:1,
              $0:doc.$0
             }:ins(doc.$0,ins(doc.$1,pos));
            }
          }
        };
        parent1=el.El;
        DomNodes.Iter(function(el1)
        {
         return DomUtility.RemoveNode(parent1,el1);
        },DomNodes.Except(DomNodes.DocChildren(el),DomNodes.Children(el.El,Runtime.GetOptional(el.Delimiters))));
        matchValue=Runtime.GetOptional(el.Delimiters);
        ins(el.Children,matchValue.$==1?{
         $:1,
         $0:matchValue.$0[1]
        }:{
         $:0
        });
        return;
       },
       DomNodes:Runtime.Class({},{
        Children:function(elem,delims)
        {
         var rdelim,ldelim,a,n,objectArg;
         if(delims.$==1)
          {
           rdelim=delims.$0[1];
           ldelim=delims.$0[0];
           a=Array.prototype.constructor.apply(Array,[]);
           n=ldelim.nextSibling;
           while(n!==rdelim)
            {
             a.push(n);
             n=n.nextSibling;
            }
           return Runtime.New(DomNodes,{
            $:0,
            $0:a
           });
          }
         else
          {
           objectArg=elem.childNodes;
           return Runtime.New(DomNodes,{
            $:0,
            $0:Arrays.init(elem.childNodes.length,function(arg00)
            {
             return objectArg[arg00];
            })
           });
          }
        },
        DocChildren:function(node)
        {
         var q,loop;
         q=[];
         loop=function(doc)
         {
          var b;
          if(doc.$==2)
           {
            return loop(doc.$0.Current);
           }
          else
           {
            if(doc.$==1)
             {
              return JQueue.Add(doc.$0.El,q);
             }
            else
             {
              if(doc.$==3)
               {
                return null;
               }
              else
               {
                if(doc.$==5)
                 {
                  return null;
                 }
                else
                 {
                  if(doc.$==4)
                   {
                    return JQueue.Add(doc.$0.Text,q);
                   }
                  else
                   {
                    b=doc.$1;
                    loop(doc.$0);
                    return loop(b);
                   }
                 }
               }
             }
           }
         };
         loop(node.Children);
         return Runtime.New(DomNodes,{
          $:0,
          $0:JQueue.ToArray(q)
         });
        },
        Except:function(_arg2,_arg1)
        {
         var excluded;
         excluded=_arg2.$0;
         return Runtime.New(DomNodes,{
          $:0,
          $0:Arrays.filter(function(n)
          {
           return Seq.forall(function(k)
           {
            return!(n===k);
           },excluded);
          },_arg1.$0)
         });
        },
        FoldBack:function(f,_arg4,z)
        {
         return Arrays.foldBack(f,_arg4.$0,z);
        },
        Iter:function(f,_arg3)
        {
         return Arrays.iter(f,_arg3.$0);
        }
       }),
       InsertDoc:function(parent,doc,pos)
       {
        var d;
        if(doc.$==1)
         {
          return Docs.InsertNode(parent,doc.$0.El,pos);
         }
        else
         {
          if(doc.$==2)
           {
            d=doc.$0;
            d.Dirty=false;
            return Docs.InsertDoc(parent,d.Current,pos);
           }
          else
           {
            return doc.$==3?pos:doc.$==4?Docs.InsertNode(parent,doc.$0.Text,pos):doc.$==5?Docs.InsertNode(parent,doc.$0,pos):Docs.InsertDoc(parent,doc.$0,Docs.InsertDoc(parent,doc.$1,pos));
           }
         }
       },
       InsertNode:function(parent,node,pos)
       {
        DomUtility.InsertAt(parent,pos,node);
        return{
         $:1,
         $0:node
        };
       },
       LinkElement:function(el,children)
       {
        Docs.InsertDoc(el,children,{
         $:0
        });
       },
       LinkPrevElement:function(el,children)
       {
        Docs.InsertDoc(el.parentNode,children,{
         $:1,
         $0:el
        });
       },
       NodeSet:Runtime.Class({},{
        Except:function(_arg3,_arg2)
        {
         return Runtime.New(NodeSet,{
          $:0,
          $0:HashSet.Except(_arg3.$0,_arg2.$0)
         });
        },
        Filter:function(f,_arg1)
        {
         return Runtime.New(NodeSet,{
          $:0,
          $0:HashSet.Filter(f,_arg1.$0)
         });
        },
        FindAll:function(doc)
        {
         var q,loop;
         q=[];
         loop=function(node)
         {
          var b,el;
          if(node.$==0)
           {
            b=node.$1;
            loop(node.$0);
            return loop(b);
           }
          else
           {
            if(node.$==1)
             {
              el=node.$0;
              JQueue.Add(el,q);
              return loop(el.Children);
             }
            else
             {
              return node.$==2?loop(node.$0.Current):null;
             }
           }
         };
         loop(doc);
         return Runtime.New(NodeSet,{
          $:0,
          $0:HashSetProxy.New(JQueue.ToArray(q))
         });
        },
        Intersect:function(_arg5,_arg4)
        {
         return Runtime.New(NodeSet,{
          $:0,
          $0:HashSet.Intersect(_arg5.$0,_arg4.$0)
         });
        },
        IsEmpty:function(_arg6)
        {
         return _arg6.$0.get_Count()===0;
        },
        ToArray:function(_arg7)
        {
         return HashSet.ToArray(_arg7.$0);
        },
        get_Empty:function()
        {
         return Runtime.New(NodeSet,{
          $:0,
          $0:HashSetProxy.New11()
         });
        }
       }),
       PerformAnimatedUpdate:function(st,doc)
       {
        return Concurrency.Delay(function()
        {
         var cur,change,enter;
         cur=NodeSet.FindAll(doc);
         change=Docs.ComputeChangeAnim(st,cur);
         enter=Docs.ComputeEnterAnim(st,cur);
         return Concurrency.Bind(An.Play(An.Append(change,Docs.ComputeExitAnim(st,cur))),function()
         {
          Docs.SyncElemNode(st.Top);
          return Concurrency.Bind(An.Play(enter),function()
          {
           return Concurrency.Return(void(st.PreviousNodes=cur));
          });
         });
        });
       },
       Sync:function(doc)
       {
        var sync;
        sync=function(doc1)
        {
         var el,d,b;
         if(doc1.$==1)
          {
           el=doc1.$0;
           Docs.SyncElement(el);
           return sync(el.Children);
          }
         else
          {
           if(doc1.$==2)
            {
             return sync(doc1.$0.Current);
            }
           else
            {
             if(doc1.$==3)
              {
               return null;
              }
             else
              {
               if(doc1.$==5)
                {
                 return null;
                }
               else
                {
                 if(doc1.$==4)
                  {
                   d=doc1.$0;
                   if(d.Dirty)
                    {
                     d.Text.nodeValue=d.Value;
                     d.Dirty=false;
                     return;
                    }
                   else
                    {
                     return null;
                    }
                  }
                 else
                  {
                   b=doc1.$1;
                   sync(doc1.$0);
                   return sync(b);
                  }
                }
              }
            }
          }
        };
        return sync(doc);
       },
       SyncElemNode:function(el)
       {
        Docs.SyncElement(el);
        return Docs.Sync(el.Children);
       },
       SyncElement:function(el)
       {
        var dirty,matchValue;
        Attrs.Sync(el.El,el.Attr);
        dirty=function(doc)
        {
         var b,d;
         if(doc.$==0)
          {
           b=doc.$1;
           return dirty(doc.$0)?true:dirty(b);
          }
         else
          {
           if(doc.$==2)
            {
             d=doc.$0;
             return d.Dirty?true:dirty(d.Current);
            }
           else
            {
             return false;
            }
          }
        };
        if(dirty(el.Children))
         {
          Docs.DoSyncElement(el);
         }
        matchValue=Runtime.GetOptional(el.Render);
        if(matchValue.$==1)
         {
          matchValue.$0.call(null,el.El);
          delete el.Render;
          return;
         }
        else
         {
          return null;
         }
       },
       UpdateEmbedNode:function(node,upd)
       {
        node.Current=upd;
        node.Dirty=true;
        return;
       },
       UpdateTextNode:function(n,t)
       {
        n.Value=t;
        n.Dirty=true;
        return;
       }
      },
      DynamicAttrNode:Runtime.Class({
       GetChangeAnim:function()
       {
        return An.get_Empty();
       },
       GetEnterAnim:function()
       {
        return An.get_Empty();
       },
       GetExitAnim:function()
       {
        return An.get_Empty();
       },
       Init:function(parent)
       {
        return this.init.call(null,parent);
       },
       Sync:function(parent)
       {
        if(this.dirty)
         {
          (this.push.call(null,parent))(this.value);
          this.dirty=false;
          return;
         }
        else
         {
          return null;
         }
       },
       get_Changed:function()
       {
        return this.updates;
       }
      },{
       New:function(view,init,push)
       {
        var r;
        r=Runtime.New(this,{});
        r.init=init;
        r.push=push;
        r.value=Abbrev.U();
        r.dirty=true;
        r.updates=View.Map(function(x)
        {
         r.value=x;
         r.dirty=true;
         return;
        },view);
        return r;
       }
      }),
      Elt:Runtime.Class({
       AddClass:function($cls)
       {
        var $this=this;
        return $this.elt.className+=" "+$cls;
       },
       Append:function(doc)
       {
        var e;
        e=this.get_DocElemNode();
        e.Children={
         $:0,
         $0:e.Children,
         $1:doc.get_DocNode()
        };
        Var1.Set(this.rvUpdates,View.Map2(function()
        {
         return function()
         {
          return null;
         };
        },Var1.Get(this.rvUpdates),doc.get_Updates()));
        Docs.InsertDoc(this.elt,doc.get_DocNode(),{
         $:0
        });
        return;
       },
       Clear:function()
       {
        this.get_DocElemNode().Children={
         $:3
        };
        Var1.Set(this.rvUpdates,View1.Const(null));
        while(this.elt.hasChildNodes())
         {
          this.elt.removeChild(this.elt.firstChild);
         }
        return;
       },
       GetAttribute:function(name)
       {
        return this.elt.getAttribute(name);
       },
       GetProperty:function(name)
       {
        return this.elt[name];
       },
       GetText:function()
       {
        return this.elt.textContent;
       },
       GetValue:function()
       {
        return this.elt.value;
       },
       HasAttribute:function(name)
       {
        return this.elt.hasAttribute(name);
       },
       HasClass:function(cls)
       {
        return(new RegExp("(\\s|^)"+cls+"(\\s|$)")).test(this.elt.className);
       },
       Html:function()
       {
        return this.elt.outerHTML;
       },
       Id:function()
       {
        return this.elt.id;
       },
       OnAfterRender:function(cb)
       {
        var matchValue,_,f;
        matchValue=Runtime.GetOptional(this.get_DocElemNode().Render);
        if(matchValue.$==1)
         {
          f=matchValue.$0;
          _={
           $:1,
           $0:function(el)
           {
            f(el);
            return cb(el);
           }
          };
         }
        else
         {
          _={
           $:1,
           $0:cb
          };
         }
        Runtime.SetOptional(this.get_DocElemNode(),"Render",_);
        return this;
       },
       Prepend:function(doc)
       {
        var e,matchValue,pos;
        e=this.get_DocElemNode();
        e.Children={
         $:0,
         $0:doc.get_DocNode(),
         $1:e.Children
        };
        Var1.Set(this.rvUpdates,View.Map2(function()
        {
         return function()
         {
          return null;
         };
        },Var1.Get(this.rvUpdates),doc.get_Updates()));
        matchValue=this.elt.firstChild;
        pos=Unchecked.Equals(matchValue,null)?{
         $:0
        }:{
         $:1,
         $0:matchValue
        };
        Docs.InsertDoc(this.elt,doc.get_DocNode(),pos);
        return;
       },
       RemoveAttribute:function(name)
       {
        return this.elt.removeAttribute(name);
       },
       RemoveClass:function(cls)
       {
        this.elt.className=this.elt.className.replace(new RegExp("(\\s|^)"+cls+"(\\s|$)")," ");
       },
       SetAttribute:function(name,value)
       {
        return this.elt.setAttribute(name,value);
       },
       SetProperty:function(name,value)
       {
        this.elt[name]=value;
       },
       SetStyle:function(style,value)
       {
        this.elt.style[style]=value;
       },
       SetText:function(v)
       {
        this.get_DocElemNode().Children={
         $:3
        };
        Var1.Set(this.rvUpdates,View1.Const(null));
        this.elt.textContent=v;
        return;
       },
       SetValue:function(v)
       {
        this.elt.value=v;
       },
       get_DocElemNode:function()
       {
        var matchValue;
        matchValue=this.docNode1;
        return matchValue.$==1?matchValue.$0:Operators.FailWith("Elt: Invalid docNode");
       },
       get_Element:function()
       {
        return this.elt;
       },
       on:function(ev,cb)
       {
        this.elt.addEventListener(ev,cb(this.elt),false);
        return this;
       }
      },{
       New:function(docNode,updates,elt,rvUpdates)
       {
        var r;
        r=Runtime.New(this,Doc.New(docNode,updates));
        r.docNode1=docNode;
        r.elt=elt;
        r.rvUpdates=rvUpdates;
        return r;
       },
       New1:function(el,attr,children)
       {
        var node,rvUpdates,attrUpdates,arg00,updates;
        node=Docs.CreateElemNode(el,attr,children.get_DocNode());
        rvUpdates=Var.Create(children.get_Updates());
        attrUpdates=Attrs.Updates(node.Attr);
        arg00=function()
        {
         return function()
         {
          return null;
         };
        };
        updates=View1.Bind(function(arg20)
        {
         return View.Map2(arg00,attrUpdates,arg20);
        },rvUpdates.get_View());
        return Elt.New({
         $:1,
         $0:node
        },updates,el,rvUpdates,attrUpdates);
       }
      })
     },
     DomUtility:{
      AddClass:function(element,cl)
      {
       jQuery(element).addClass(cl);
      },
      AppendTo:function(ctx,node)
      {
       ctx.appendChild(node);
      },
      Clear:function(ctx)
      {
       while(ctx.hasChildNodes())
        {
         ctx.removeChild(ctx.firstChild);
        }
       return;
      },
      ClearAttrs:function(ctx)
      {
       while(ctx.hasAttributes())
        {
         ctx.removeAttributeNode(ctx.attributes.item(0));
        }
       return;
      },
      CreateAttr:function(name,value)
      {
       var a;
       a=DomUtility.Doc().createAttribute(name);
       a.value=value;
       return a;
      },
      CreateElement:function(name)
      {
       return DomUtility.Doc().createElement(name);
      },
      CreateSvgElement:function(name)
      {
       return DomUtility.Doc().createElementNS("http://www.w3.org/2000/svg",name);
      },
      CreateText:function(s)
      {
       return DomUtility.Doc().createTextNode(s);
      },
      Doc:Runtime.Field(function()
      {
       return document;
      }),
      InsertAt:function(parent,pos,node)
      {
       var _,matchValue,matchValue1;
       if(node.parentNode===parent)
        {
         matchValue=node.nextSibling;
         matchValue1=[pos,Unchecked.Equals(matchValue,null)?{
          $:0
         }:{
          $:1,
          $0:matchValue
         }];
         _=matchValue1[0].$==1?matchValue1[1].$==1?matchValue1[0].$0===matchValue1[1].$0:false:matchValue1[1].$==0?true:false;
        }
       else
        {
         _=false;
        }
       return!_?pos.$==1?void parent.insertBefore(node,pos.$0):void parent.appendChild(node):null;
      },
      RemoveAttr:function(el,attrName)
      {
       return el.removeAttribute(attrName);
      },
      RemoveClass:function(element,cl)
      {
       jQuery(element).removeClass(cl);
      },
      RemoveNode:function(parent,el)
      {
       return el.parentNode===parent?void parent.removeChild(el):null;
      },
      SetAttr:function(el,name,value)
      {
       return el.setAttribute(name,value);
      },
      SetProperty:function($target,$name,$value)
      {
       var $0=this,$this=this;
       return $target.setProperty($name,$value);
      },
      SetStyle:function(el,name,value)
      {
       return DomUtility.SetProperty(el.style,name,value);
      }
     },
     DoubleInterpolation:Runtime.Class({
      Interpolate:function(t,x,y)
      {
       return x+t*(y-x);
      }
     }),
     Easing:Runtime.Class({},{
      Custom:function(f)
      {
       return Runtime.New(Easing,{
        TransformTime:f
       });
      },
      get_CubicInOut:function()
      {
       return Easings.CubicInOut();
      }
     }),
     Easings:{
      CubicInOut:Runtime.Field(function()
      {
       return Runtime.New(Easing,{
        TransformTime:function(t)
        {
         var t2;
         t2=t*t;
         return 3*t2-2*(t2*t);
        }
       });
      })
     },
     Flow1:Runtime.Class({},{
      Bind:function(m,k)
      {
       return{
        Render:function(_var)
        {
         return function(cont)
         {
          return(m.Render.call(null,_var))(function(r)
          {
           return(k(r).Render.call(null,_var))(cont);
          });
         };
        }
       };
      },
      Define:function(f)
      {
       return{
        Render:function(_var)
        {
         return function(cont)
         {
          return Var1.Set(_var,f(cont));
         };
        }
       };
      },
      Embed:function(fl)
      {
       var _var;
       _var=Var.Create(Doc.get_Empty());
       (fl.Render.call(null,_var))(function()
       {
       });
       return Doc.EmbedView(_var.get_View());
      },
      Map:function(f,x)
      {
       return{
        Render:function(_var)
        {
         return function(cont)
         {
          return(x.Render.call(null,_var))(function(r)
          {
           return cont(f(r));
          });
         };
        }
       };
      },
      Return:function(x)
      {
       return{
        Render:function()
        {
         return function(cont)
         {
          return cont(x);
         };
        }
       };
      },
      Static:function(doc)
      {
       return{
        Render:function(_var)
        {
         return function(cont)
         {
          Var1.Set(_var,doc);
          return cont(null);
         };
        }
       };
      },
      get_Do:function()
      {
       return FlowBuilder.New();
      }
     }),
     FlowBuilder:Runtime.Class({
      Bind:function(comp,func)
      {
       return Flow1.Bind(comp,func);
      },
      Return:function(value)
      {
       return Flow1.Return(value);
      },
      ReturnFrom:function(inner)
      {
       return inner;
      }
     },{
      New:function()
      {
       return Runtime.New(this,{});
      }
     }),
     Html:{
      attr:Runtime.Class({},{
       New:function()
       {
        return Runtime.New(this,{});
       }
      })
     },
     Input:{
      ActivateButtonListener:Runtime.Field(function()
      {
       var _buttonListener_39_1,_;
       _buttonListener_39_1=function(evt,down)
       {
        var matchValue;
        matchValue=evt.button;
        return matchValue===0?Var1.Set(Input.MouseBtnSt1().Left,down):matchValue===1?Var1.Set(Input.MouseBtnSt1().Middle,down):matchValue===2?Var1.Set(Input.MouseBtnSt1().Right,down):null;
       };
       if(!Input.MouseBtnSt1().Active)
        {
         Input.MouseBtnSt1().Active=true;
         document.addEventListener("mousedown",function(evt)
         {
          return _buttonListener_39_1(evt,true);
         },false);
         _=document.addEventListener("mouseup",function(evt)
         {
          return _buttonListener_39_1(evt,false);
         },false);
        }
       else
        {
         _=null;
        }
       return _;
      }),
      ActivateKeyListener:Runtime.Field(function()
      {
       var _;
       if(!Input.KeyListenerState().KeyListenerActive)
        {
         jQuery(document).keydown(function(evt)
         {
          var keyCode,xs;
          keyCode=evt.which;
          Var1.Set(Input.KeyListenerState().LastPressed,keyCode);
          xs=Var1.Get(Input.KeyListenerState().KeysPressed);
          return!Seq.exists(function(x)
          {
           return x===keyCode;
          },xs)?Var1.Set(Input.KeyListenerState().KeysPressed,List.append(xs,List.ofArray([keyCode]))):null;
         });
         _=void jQuery(document).keyup(function(evt)
         {
          var keyCode,predicate,arg10;
          keyCode=evt.which;
          predicate=function(x)
          {
           return x!==keyCode;
          };
          arg10=function(list)
          {
           return List.filter(predicate,list);
          };
          return Var1.Update(Input.KeyListenerState().KeysPressed,arg10);
         });
        }
       else
        {
         _=null;
        }
       return _;
      }),
      KeyListenerState:Runtime.Field(function()
      {
       return{
        KeysPressed:Var.Create(Runtime.New(T,{
         $:0
        })),
        KeyListenerActive:false,
        LastPressed:Var.Create(-1)
       };
      }),
      Keyboard:Runtime.Class({},{
       IsPressed:function(key)
       {
        var predicate;
        Input.ActivateKeyListener();
        predicate=function(x)
        {
         return x===key;
        };
        return View.Map(function(list)
        {
         return Seq.exists(predicate,list);
        },Input.KeyListenerState().KeysPressed.get_View());
       },
       get_KeysPressed:function()
       {
        Input.ActivateKeyListener();
        return Input.KeyListenerState().KeysPressed.get_View();
       },
       get_LastPressed:function()
       {
        Input.ActivateKeyListener();
        return Input.KeyListenerState().LastPressed.get_View();
       }
      }),
      Mouse:Runtime.Class({},{
       get_LeftPressed:function()
       {
        Input.ActivateButtonListener();
        return Input.MouseBtnSt1().Left.get_View();
       },
       get_MiddlePressed:function()
       {
        Input.ActivateButtonListener();
        return Input.MouseBtnSt1().Middle.get_View();
       },
       get_MousePressed:function()
       {
        Input.ActivateButtonListener();
        return View1.Apply(View1.Apply(View1.Apply(View1.Const(function(l)
        {
         return function(m)
         {
          return function(r)
          {
           return(l?true:m)?true:r;
          };
         };
        }),Input.MouseBtnSt1().Left.get_View()),Input.MouseBtnSt1().Middle.get_View()),Input.MouseBtnSt1().Right.get_View());
       },
       get_Position:function()
       {
        var onMouseMove;
        onMouseMove=function(evt)
        {
         return Var1.Set(Input.MousePosSt1().PosV,[evt.clientX,evt.clientY]);
        };
        if(!Input.MousePosSt1().Active)
         {
          document.addEventListener("mousemove",onMouseMove,false);
          Input.MousePosSt1().Active=true;
         }
        return Input.MousePosSt1().PosV.get_View();
       },
       get_RightPressed:function()
       {
        Input.ActivateButtonListener();
        return Input.MouseBtnSt1().Right.get_View();
       }
      }),
      MouseBtnSt1:Runtime.Field(function()
      {
       return{
        Active:false,
        Left:Var.Create(false),
        Middle:Var.Create(false),
        Right:Var.Create(false)
       };
      }),
      MousePosSt1:Runtime.Field(function()
      {
       return{
        Active:false,
        PosV:Var.Create([0,0])
       };
      })
     },
     Interpolation1:Runtime.Class({},{
      get_Double:function()
      {
       return Runtime.New(DoubleInterpolation,{
        $:0
       });
      }
     }),
     Key:Runtime.Class({},{
      Fresh:function()
      {
       return Runtime.New(Key,{
        $:0,
        $0:Fresh.Int()
       });
      }
     }),
     ListModel:Runtime.Class({
      Add:function(item)
      {
       var v,m=this;
       v=Var1.Get(this.Var);
       if(!ListModels.Contains(this.key,item,v))
        {
         v.push(item);
         return Var1.Set(this.Var,v);
        }
       else
        {
         Arrays.set(v,Arrays.findINdex(function(it)
         {
          return Unchecked.Equals(m.key.call(null,it),m.key.call(null,item));
         },v),item);
         return Var1.Set(m.Var,v);
        }
      },
      Clear:function()
      {
       return Var1.Set(this.Var,[]);
      },
      ContainsKey:function(key)
      {
       var m=this;
       return Seq.exists(function(it)
       {
        return Unchecked.Equals(m.key.call(null,it),key);
       },Var1.Get(m.Var));
      },
      ContainsKeyAsView:function(key)
      {
       var predicate,m=this;
       predicate=function(it)
       {
        return Unchecked.Equals(m.key.call(null,it),key);
       };
       return View.Map(function(array)
       {
        return Seq.exists(predicate,array);
       },m.Var.get_View());
      },
      Find:function(pred)
      {
       return Arrays.find(pred,Var1.Get(this.Var));
      },
      FindAsView:function(pred)
      {
       return View.Map(function(array)
       {
        return Arrays.find(pred,array);
       },this.Var.get_View());
      },
      FindByKey:function(key)
      {
       var m=this;
       return Arrays.find(function(it)
       {
        return Unchecked.Equals(m.key.call(null,it),key);
       },Var1.Get(m.Var));
      },
      FindByKeyAsView:function(key)
      {
       var predicate,m=this;
       predicate=function(it)
       {
        return Unchecked.Equals(m.key.call(null,it),key);
       };
       return View.Map(function(array)
       {
        return Arrays.find(predicate,array);
       },m.Var.get_View());
      },
      Iter:function(fn)
      {
       return Arrays.iter(fn,Var1.Get(this.Var));
      },
      Lens:function(key)
      {
       return RefImpl1.New(this,key,function(x)
       {
        return x;
       },function()
       {
        return function(x)
        {
         return x;
        };
       });
      },
      Remove:function(item)
      {
       var v,keyFn,k;
       v=Var1.Get(this.Var);
       if(ListModels.Contains(this.key,item,v))
        {
         keyFn=this.key;
         k=keyFn(item);
         return Var1.Set(this.Var,Arrays.filter(function(i)
         {
          return!Unchecked.Equals(keyFn(i),k);
         },v));
        }
       else
        {
         return null;
        }
      },
      RemoveBy:function(f)
      {
       return Var1.Set(this.Var,Arrays.filter(function(x)
       {
        return!f(x);
       },Var1.Get(this.Var)));
      },
      RemoveByKey:function(key)
      {
       var m=this;
       return Var1.Set(this.Var,Arrays.filter(function(i)
       {
        return!Unchecked.Equals(m.key.call(null,i),key);
       },Var1.Get(m.Var)));
      },
      Set:function(lst)
      {
       return Var1.Set(this.Var,Arrays.ofSeq(lst));
      },
      TryFind:function(pred)
      {
       return Arrays.tryFind(pred,Var1.Get(this.Var));
      },
      TryFindAsView:function(pred)
      {
       return View.Map(function(array)
       {
        return Arrays.tryFind(pred,array);
       },this.Var.get_View());
      },
      TryFindByKey:function(key)
      {
       var m=this;
       return Arrays.tryFind(function(it)
       {
        return Unchecked.Equals(m.key.call(null,it),key);
       },Var1.Get(m.Var));
      },
      TryFindByKeyAsView:function(key)
      {
       var predicate,m=this;
       predicate=function(it)
       {
        return Unchecked.Equals(m.key.call(null,it),key);
       };
       return View.Map(function(array)
       {
        return Arrays.tryFind(predicate,array);
       },m.Var.get_View());
      },
      UpdateAll:function(fn)
      {
       return Var1.Update(this.Var,function(a)
       {
        Arrays.iteri(function(i)
        {
         return function(x)
         {
          return Option.iter(function(y)
          {
           return Arrays.set(a,i,y);
          },fn(x));
         };
        },a);
        return a;
       });
      },
      UpdateBy:function(fn,key)
      {
       var v,matchValue,m=this,index,matchValue1;
       v=Var1.Get(this.Var);
       matchValue=Arrays.tryFindIndex(function(it)
       {
        return Unchecked.Equals(m.key.call(null,it),key);
       },v);
       if(matchValue.$==1)
        {
         index=matchValue.$0;
         matchValue1=fn(Arrays.get(v,index));
         if(matchValue1.$==1)
          {
           Arrays.set(v,index,matchValue1.$0);
           return Var1.Set(m.Var,v);
          }
         else
          {
           return null;
          }
        }
       else
        {
         return null;
        }
      },
      get_Key:function()
      {
       return this.key;
      },
      get_Length:function()
      {
       return Arrays.length(Var1.Get(this.Var));
      },
      get_LengthAsView:function()
      {
       return View.Map(function(arr)
       {
        return Arrays.length(arr);
       },this.Var.get_View());
      },
      get_View:function()
      {
       return this.view;
      }
     },{
      Create:function(key,init)
      {
       var _var;
       _var=Var.Create(Seq.toArray(Seq1.distinctBy(key,init)));
       return Runtime.New(ListModel,{
        key:key,
        Var:_var,
        view:View.Map(function(x)
        {
         return x.slice();
        },_var.get_View())
       });
      },
      FromSeq:function(xs)
      {
       return ListModel.Create(function(x)
       {
        return x;
       },xs);
      }
     }),
     ListModel1:Runtime.Class({},{
      Key:function(m)
      {
       return m.key;
      },
      View:function(m)
      {
       return m.view;
      }
     }),
     ListModels:{
      Contains:function(keyFn,item,xs)
      {
       var t;
       t=keyFn(item);
       return Seq.exists(function(it)
       {
        return Unchecked.Equals(keyFn(it),t);
       },xs);
      }
     },
     Model:Runtime.Class({
      get_View:function()
      {
       return Model1.View(this);
      }
     }),
     Model1:Runtime.Class({},{
      Create:function(proj,init)
      {
       var _var;
       _var=Var.Create(init);
       return Runtime.New(Model,{
        $:0,
        $0:_var,
        $1:View.Map(proj,_var.get_View())
       });
      },
      Update:function(update,_arg1)
      {
       return Var1.Update(_arg1.$0,function(x)
       {
        update(x);
        return x;
       });
      },
      View:function(_arg2)
      {
       return _arg2.$1;
      }
     }),
     RefImpl:Runtime.Class({
      Get:function()
      {
       return this.get.call(null,this.baseRef.Get());
      },
      Set:function(v)
      {
       var _this=this;
       return this.baseRef.Update(function(t)
       {
        return(_this.update.call(null,t))(v);
       });
      },
      Update:function(f)
      {
       var _this=this;
       return this.baseRef.Update(function(t)
       {
        return(_this.update.call(null,t))(f(_this.get.call(null,t)));
       });
      },
      UpdateMaybe:function(f)
      {
       var _this=this;
       return this.baseRef.UpdateMaybe(function(t)
       {
        return Option.map(_this.update.call(null,t),f(_this.get.call(null,t)));
       });
      },
      get_Id:function()
      {
       return this.id;
      },
      get_View:function()
      {
       return View.Map(this.get,this.baseRef.get_View());
      }
     },{
      New:function(baseRef,get,update)
      {
       var r;
       r=Runtime.New(this,{});
       r.baseRef=baseRef;
       r.get=get;
       r.update=update;
       r.id=Fresh.Id();
       return r;
      }
     }),
     RefImpl1:Runtime.Class({
      Get:function()
      {
       return this.get.call(null,this.m.FindByKey(this.key));
      },
      Set:function(v)
      {
       var r=this;
       return this.m.UpdateBy(function(i)
       {
        return{
         $:1,
         $0:(r.update.call(null,i))(v)
        };
       },r.key);
      },
      Update:function(f)
      {
       var r=this;
       return this.m.UpdateBy(function(i)
       {
        return{
         $:1,
         $0:(r.update.call(null,i))(f(r.get.call(null,i)))
        };
       },r.key);
      },
      UpdateMaybe:function(f)
      {
       var r=this;
       return this.m.UpdateBy(function(i)
       {
        return Option.map(r.update.call(null,i),f(r.get.call(null,i)));
       },r.key);
      },
      get_Id:function()
      {
       return this.id;
      },
      get_View:function()
      {
       return View.Map(this.get,this.m.FindByKeyAsView(this.key));
      }
     },{
      New:function(m,key,get,update)
      {
       var r;
       r=Runtime.New(this,{});
       r.m=m;
       r.key=key;
       r.get=get;
       r.update=update;
       r.id=Fresh.Id();
       return r;
      }
     }),
     Route:{
      Append:function(_arg2,_arg1)
      {
       return{
        $:0,
        $0:AppendList1.Append(_arg2.$0,_arg1.$0)
       };
      },
      FromList:function(xs)
      {
       return{
        $:0,
        $0:AppendList1.FromArray(Arrays.ofSeq(xs))
       };
      },
      MakeHash:function(_arg1)
      {
       return Strings.concat("/",Arrays.map(function(x)
       {
        return encodeURIComponent(x);
       },AppendList1.ToArray(_arg1.$0)));
      },
      NoHash:function(s)
      {
       return Strings.StartsWith(s,"#")?s.substring(1):s;
      },
      ParseHash:function(hash)
      {
       return{
        $:0,
        $0:AppendList1.FromArray(Arrays.map(function(x)
        {
         return decodeURIComponent(x);
        },Strings.SplitChars(Route.NoHash(hash),[47],1)))
       };
      },
      SameHash:function(a,b)
      {
       return Route.NoHash(a)===Route.NoHash(b);
      },
      ToList:function(_arg1)
      {
       return List.ofArray(AppendList1.ToArray(_arg1.$0));
      }
     },
     RouteMap:Runtime.Class({},{
      Create:function(ser,des)
      {
       return{
        Des:des,
        Ser:ser
       };
      }
     }),
     RouteMap1:Runtime.Class({},{
      Install:function(map)
      {
       return Routing.InstallMap(map);
      }
     }),
     Router1:Runtime.Class({},{
      Dir:function(prefix,sites)
      {
       return Router1.Prefix(prefix,Router1.Merge(sites));
      },
      Install:function(key,site)
      {
       return Routing.Install(key,site);
      },
      Merge:function(sites)
      {
       return Routing.MergeRouters(sites);
      },
      Prefix:function(prefix,_arg1)
      {
       return{
        $:0,
        $0:_arg1.$0,
        $1:Trie1.Prefix(prefix,_arg1.$1)
       };
      },
      Route:function(r,init,render)
      {
       return Routing.DefineRoute(r,init,render);
      }
     }),
     Routing:{
      ComputeBodies:function(trie)
      {
       var d;
       d=Dictionary.New12();
       Arrays.iter(function(body)
       {
        return d.set_Item(body.RouteId,body);
       },Trie1.ToArray(trie));
       return d;
      },
      DefineRoute:function(r,init,render)
      {
       var state,id,site,t;
       state=Var.Create(init);
       id=Fresh.Int();
       site=(render({
        $:0,
        $0:id
       }))(state);
       t=Trie1.Leaf({
        $:0,
        $0:id,
        $1:function(ctx)
        {
         View1.Sink(function(va)
         {
          return ctx.UpdateRoute.call(null,Routing.DoLink(r,va));
         },state.get_View());
         return{
          OnRouteChanged:function(route)
          {
           return Var1.Set(state,Routing.DoRoute(r,route));
          },
          OnSelect:function()
          {
           return ctx.UpdateRoute.call(null,Routing.DoLink(r,Var1.Get(state)));
          },
          RouteId:id,
          RouteValue:site
         };
        }
       });
       return{
        $:0,
        $0:{
         $:1,
         $0:site
        },
        $1:t
       };
      },
      DoLink:function(map,va)
      {
       return Route.FromList(map.Ser.call(null,va));
      },
      DoRoute:function(map,route)
      {
       return map.Des.call(null,Route.ToList(route));
      },
      Install:function(key,_arg1)
      {
       var va,site,currentRoute,state,siteTrie,parseRoute,matchValue,glob,site1,updateRoute;
       va=_arg1.$0;
       site=_arg1.$1;
       currentRoute=Routing.InstallMap({
        Des:function(xs)
        {
         return Route.FromList(xs);
        },
        Ser:function(_arg00_)
        {
         return Route.ToList(_arg00_);
        }
       });
       state={
        Bodies:Abbrev.U(),
        CurrentRoute:currentRoute,
        CurrentSite:0,
        Selection:Abbrev.U()
       };
       siteTrie=Trie1.Map(function(prefix)
       {
        return function(_arg11)
        {
         var id;
         id=_arg11.$0;
         return _arg11.$1.call(null,{
          UpdateRoute:function(rest)
          {
           return Routing.OnInternalSiteUpdate(state,id,prefix,rest);
          }
         });
        };
       },site);
       state.Bodies=Routing.ComputeBodies(siteTrie);
       parseRoute=function(route)
       {
        return Trie1.Lookup(siteTrie,Route.ToList(route));
       };
       matchValue=parseRoute(Var1.Get(currentRoute));
       if(matchValue.$==0)
        {
         site1=matchValue.$0;
         state.CurrentSite=site1.RouteId;
         glob=Var.Create(site1.RouteValue);
        }
       else
        {
         glob=Var.Create(va.$==1?va.$0:Operators.FailWith("Site.Install fails on empty site"));
        }
       state.Selection=glob;
       View1.Sink(function(site2)
       {
        return Routing.OnSelectSite(state,key(site2));
       },glob.get_View());
       updateRoute=function(route)
       {
        var matchValue1;
        matchValue1=parseRoute(route);
        return matchValue1.$==1?null:Routing.OnGlobalRouteChange(state,matchValue1.$0,Route.FromList(matchValue1.$1));
       };
       updateRoute(Var1.Get(currentRoute));
       View1.Sink(updateRoute,currentRoute.get_View());
       return glob;
      },
      InstallMap:function(rt)
      {
       var cur,_var,onUpdate;
       cur=function()
       {
        return rt.Des.call(null,Route.ToList(Route.ParseHash(window.location.hash)));
       };
       _var=Var.Create(cur(null));
       onUpdate=function()
       {
        var value;
        value=cur(null);
        return!Unchecked.Equals(rt.Ser.call(null,Var1.Get(_var)),rt.Ser.call(null,value))?Var1.Set(_var,value):null;
       };
       window.onpopstate=onUpdate;
       window.onhashchange=onUpdate;
       View1.Sink(function(loc)
       {
        var ha;
        ha=Route.MakeHash(Route.FromList(rt.Ser.call(null,loc)));
        return!Route.SameHash(window.location.hash,ha)?void(window.location.hash=ha):null;
       },_var.get_View());
       return _var;
      },
      MergeRouters:function(sites)
      {
       var sites1,merged,value;
       sites1=Seq.toArray(sites);
       merged=Trie1.Merge(Seq.map(function(_arg1)
       {
        return _arg1.$1;
       },sites1));
       value=Seq.tryPick(function(_arg2)
       {
        return _arg2.$0;
       },sites1);
       return merged.$==1?{
        $:0,
        $0:value,
        $1:merged.$0
       }:Operators.FailWith("Invalid Site.Merge: need more prefix disambiguation");
      },
      OnGlobalRouteChange:function(state,site,rest)
      {
       if(state.CurrentSite!==site.RouteId)
        {
         state.CurrentSite=site.RouteId;
         Var1.Set(state.Selection,site.RouteValue);
        }
       return site.OnRouteChanged.call(null,rest);
      },
      OnInternalSiteUpdate:function(state,ix,prefix,rest)
      {
       return state.CurrentSite===ix?Routing.SetCurrentRoute(state,Route.Append(Route.FromList(prefix),rest)):null;
      },
      OnSelectSite:function(state,_arg1)
      {
       var id;
       id=_arg1.$0;
       if(state.CurrentSite!==id)
        {
         state.CurrentSite=id;
         return state.Bodies.get_Item(id).OnSelect.call(null,null);
        }
       else
        {
         return null;
        }
      },
      SetCurrentRoute:function(state,route)
      {
       return!Unchecked.Equals(Var1.Get(state.CurrentRoute),route)?Var1.Set(state.CurrentRoute,route):null;
      }
     },
     Snap1:{
      Bind:function(f,snap)
      {
       var res,onObs;
       res=Snap1.Create();
       onObs=function()
       {
        return Snap1.MarkObsolete(res);
       };
       Snap1.When(snap,function(x)
       {
        var y;
        y=f(x);
        return Snap1.When(y,function(v)
        {
         return(Snap1.IsForever(y)?Snap1.IsForever(snap):false)?Snap1.MarkForever(res,v):Snap1.MarkReady(res,v);
        },onObs);
       },onObs);
       return res;
      },
      Create:function()
      {
       return Snap1.Make({
        $:3,
        $0:[],
        $1:[]
       });
      },
      CreateForever:function(v)
      {
       return Snap1.Make({
        $:0,
        $0:v
       });
      },
      CreateWithValue:function(v)
      {
       return Snap1.Make({
        $:2,
        $0:v,
        $1:[]
       });
      },
      IsForever:function(snap)
      {
       return snap.State.$==0?true:false;
      },
      IsObsolete:function(snap)
      {
       return snap.State.$==1?true:false;
      },
      Make:function(st)
      {
       return{
        State:st
       };
      },
      Map:function(fn,sn)
      {
       var matchValue,res;
       matchValue=sn.State;
       if(matchValue.$==0)
        {
         return Snap1.CreateForever(fn(matchValue.$0));
        }
       else
        {
         res=Snap1.Create();
         Snap1.When(sn,function(x)
         {
          return Snap1.MarkDone(res,sn,fn(x));
         },function()
         {
          return Snap1.MarkObsolete(res);
         });
         return res;
        }
      },
      Map2:function(fn,sn1,sn2)
      {
       var matchValue,y,y1,res,v1,v2,obs,cont;
       matchValue=[sn1.State,sn2.State];
       if(matchValue[0].$==0)
        {
         if(matchValue[1].$==0)
          {
           y=matchValue[1].$0;
           return Snap1.CreateForever((fn(matchValue[0].$0))(y));
          }
         else
          {
           return Snap1.Map(fn(matchValue[0].$0),sn2);
          }
        }
       else
        {
         if(matchValue[1].$==0)
          {
           y1=matchValue[1].$0;
           return Snap1.Map(function(x)
           {
            return(fn(x))(y1);
           },sn1);
          }
         else
          {
           res=Snap1.Create();
           v1=[{
            $:0
           }];
           v2=[{
            $:0
           }];
           obs=function()
           {
            v1[0]={
             $:0
            };
            v2[0]={
             $:0
            };
            return Snap1.MarkObsolete(res);
           };
           cont=function()
           {
            var matchValue1,x,y2;
            matchValue1=[v1[0],v2[0]];
            if(matchValue1[0].$==1)
             {
              if(matchValue1[1].$==1)
               {
                x=matchValue1[0].$0;
                y2=matchValue1[1].$0;
                return(Snap1.IsForever(sn1)?Snap1.IsForever(sn2):false)?Snap1.MarkForever(res,(fn(x))(y2)):Snap1.MarkReady(res,(fn(x))(y2));
               }
              else
               {
                return null;
               }
             }
            else
             {
              return null;
             }
           };
           Snap1.When(sn1,function(x)
           {
            v1[0]={
             $:1,
             $0:x
            };
            return cont(null);
           },obs);
           Snap1.When(sn2,function(y2)
           {
            v2[0]={
             $:1,
             $0:y2
            };
            return cont(null);
           },obs);
           return res;
          }
        }
      },
      Map3:function(fn,sn1,sn2,sn3)
      {
       var matchValue,y,z,x,y1,x1,z2,x2,y3,z3,y4,z4,res,v1,v2,v3,obs,cont;
       matchValue=[sn1.State,sn2.State,sn3.State];
       if(matchValue[0].$==0)
        {
         if(matchValue[1].$==0)
          {
           if(matchValue[2].$==0)
            {
             y=matchValue[1].$0;
             z=matchValue[2].$0;
             return Snap1.CreateForever(((fn(matchValue[0].$0))(y))(z));
            }
           else
            {
             x=matchValue[0].$0;
             y1=matchValue[1].$0;
             return Snap1.Map(function(z1)
             {
              return((fn(x))(y1))(z1);
             },sn3);
            }
          }
         else
          {
           if(matchValue[2].$==0)
            {
             x1=matchValue[0].$0;
             z2=matchValue[2].$0;
             return Snap1.Map(function(y2)
             {
              return((fn(x1))(y2))(z2);
             },sn2);
            }
           else
            {
             x2=matchValue[0].$0;
             return Snap1.Map2(function(y2)
             {
              return function(z1)
              {
               return((fn(x2))(y2))(z1);
              };
             },sn2,sn3);
            }
          }
        }
       else
        {
         if(matchValue[1].$==0)
          {
           if(matchValue[2].$==0)
            {
             y3=matchValue[1].$0;
             z3=matchValue[2].$0;
             return Snap1.Map(function(x3)
             {
              return((fn(x3))(y3))(z3);
             },sn1);
            }
           else
            {
             y4=matchValue[1].$0;
             return Snap1.Map2(function(x3)
             {
              return function(z1)
              {
               return((fn(x3))(y4))(z1);
              };
             },sn1,sn3);
            }
          }
         else
          {
           if(matchValue[2].$==0)
            {
             z4=matchValue[2].$0;
             return Snap1.Map2(function(x3)
             {
              return function(y2)
              {
               return((fn(x3))(y2))(z4);
              };
             },sn1,sn2);
            }
           else
            {
             res=Snap1.Create();
             v1=[{
              $:0
             }];
             v2=[{
              $:0
             }];
             v3=[{
              $:0
             }];
             obs=function()
             {
              v1[0]={
               $:0
              };
              v2[0]={
               $:0
              };
              v3[0]={
               $:0
              };
              return Snap1.MarkObsolete(res);
             };
             cont=function()
             {
              var matchValue1,x3,y2,z1;
              matchValue1=[v1[0],v2[0],v3[0]];
              if(matchValue1[0].$==1)
               {
                if(matchValue1[1].$==1)
                 {
                  if(matchValue1[2].$==1)
                   {
                    x3=matchValue1[0].$0;
                    y2=matchValue1[1].$0;
                    z1=matchValue1[2].$0;
                    return((Snap1.IsForever(sn1)?Snap1.IsForever(sn2):false)?Snap1.IsForever(sn3):false)?Snap1.MarkForever(res,((fn(x3))(y2))(z1)):Snap1.MarkReady(res,((fn(x3))(y2))(z1));
                   }
                  else
                   {
                    return null;
                   }
                 }
                else
                 {
                  return null;
                 }
               }
              else
               {
                return null;
               }
             };
             Snap1.When(sn1,function(x3)
             {
              v1[0]={
               $:1,
               $0:x3
              };
              return cont(null);
             },obs);
             Snap1.When(sn2,function(y2)
             {
              v2[0]={
               $:1,
               $0:y2
              };
              return cont(null);
             },obs);
             Snap1.When(sn3,function(z1)
             {
              v3[0]={
               $:1,
               $0:z1
              };
              return cont(null);
             },obs);
             return res;
            }
          }
        }
      },
      MapAsync:function(fn,snap)
      {
       var res;
       res=Snap1.Create();
       Snap1.When(snap,function(v)
       {
        return Async.StartTo(fn(v),function(v1)
        {
         return Snap1.MarkDone(res,snap,v1);
        });
       },function()
       {
        return Snap1.MarkObsolete(res);
       });
       return res;
      },
      MapCached:function(prev,fn,sn)
      {
       return Snap1.Map(function(x)
       {
        var matchValue,y,y1;
        matchValue=prev[0];
        if(matchValue.$==1)
         {
          if(Unchecked.Equals(x,matchValue.$0[0]))
           {
            return matchValue.$0[1];
           }
          else
           {
            y=fn(x);
            prev[0]={
             $:1,
             $0:[x,y]
            };
            return y;
           }
         }
        else
         {
          y1=fn(x);
          prev[0]={
           $:1,
           $0:[x,y1]
          };
          return y1;
         }
       },sn);
      },
      MarkDone:function(res,sn,v)
      {
       return Snap1.IsForever(sn)?Snap1.MarkForever(res,v):Snap1.MarkReady(res,v);
      },
      MarkForever:function(sn,v)
      {
       var matchValue,q;
       matchValue=sn.State;
       if(matchValue.$==3)
        {
         q=matchValue.$0;
         sn.State={
          $:0,
          $0:v
         };
         return JQueue.Iter(function(k)
         {
          return k(v);
         },q);
        }
       else
        {
         return null;
        }
      },
      MarkObsolete:function(sn)
      {
       var matchValue,ks,ks1;
       matchValue=sn.State;
       if(matchValue.$==1)
        {
         return null;
        }
       else
        {
         if(matchValue.$==2)
          {
           ks=matchValue.$1;
           sn.State={
            $:1
           };
           return JQueue.Iter(function(k)
           {
            return k(null);
           },ks);
          }
         else
          {
           if(matchValue.$==3)
            {
             ks1=matchValue.$1;
             sn.State={
              $:1
             };
             return JQueue.Iter(function(k)
             {
              return k(null);
             },ks1);
            }
           else
            {
             return null;
            }
          }
        }
      },
      MarkReady:function(sn,v)
      {
       var matchValue,q1;
       matchValue=sn.State;
       if(matchValue.$==3)
        {
         q1=matchValue.$0;
         sn.State={
          $:2,
          $0:v,
          $1:matchValue.$1
         };
         return JQueue.Iter(function(k)
         {
          return k(v);
         },q1);
        }
       else
        {
         return null;
        }
      },
      SnapshotOn:function(sn1,sn2)
      {
       var matchValue,res,v,triggered,cont;
       matchValue=[sn1.State,sn2.State];
       if(matchValue[1].$==0)
        {
         return Snap1.CreateForever(matchValue[1].$0);
        }
       else
        {
         res=Snap1.Create();
         v=[{
          $:0
         }];
         triggered=[false];
         cont=function()
         {
          var matchValue1;
          if(triggered[0])
           {
            matchValue1=v[0];
            return matchValue1.$==1?Snap1.IsForever(sn2)?Snap1.MarkForever(res,matchValue1.$0):matchValue1.$==1?Snap1.MarkReady(res,matchValue1.$0):null:matchValue1.$==1?Snap1.MarkReady(res,matchValue1.$0):null;
           }
          else
           {
            return null;
           }
         };
         Snap1.When(sn1,function()
         {
          triggered[0]=true;
          return cont(null);
         },function()
         {
          v[0]={
           $:0
          };
          return Snap1.MarkObsolete(res);
         });
         Snap1.When(sn2,function(y)
         {
          v[0]={
           $:1,
           $0:y
          };
          return cont(null);
         },function()
         {
         });
         return res;
        }
      },
      When:function(snap,avail,obsolete)
      {
       var matchValue,v,q2;
       matchValue=snap.State;
       if(matchValue.$==1)
        {
         return obsolete(null);
        }
       else
        {
         if(matchValue.$==2)
          {
           v=matchValue.$0;
           JQueue.Add(obsolete,matchValue.$1);
           return avail(v);
          }
         else
          {
           if(matchValue.$==3)
            {
             q2=matchValue.$1;
             JQueue.Add(avail,matchValue.$0);
             return JQueue.Add(obsolete,q2);
            }
           else
            {
             return avail(matchValue.$0);
            }
          }
        }
      }
     },
     String:{
      isBlank:function(s)
      {
       return Strings.forall(function(arg00)
       {
        return Char.IsWhiteSpace(arg00);
       },s);
      }
     },
     Submitter:Runtime.Class({
      Trigger:function()
      {
       return Var1.Set(this["var"],null);
      },
      get_Input:function()
      {
       return this.input;
      },
      get_View:function()
      {
       return this.view;
      }
     },{
      Create:function(input,init)
      {
       return Submitter.New(input,init);
      },
      Input:function(s)
      {
       return s.get_Input();
      },
      New:function(input,init)
      {
       var r,arg20;
       r=Runtime.New(this,{});
       r.input=input;
       r["var"]=Var.Create(null);
       arg20=r.input;
       r.view=View.SnapshotOn(init,r["var"].get_View(),arg20);
       return r;
      },
      Trigger:function(s)
      {
       return s.Trigger();
      },
      View:function(s)
      {
       return s.get_View();
      }
     }),
     Trans:Runtime.Class({},{
      AnimateChange:function(tr,x,y)
      {
       return(tr.TChange.call(null,x))(y);
      },
      AnimateEnter:function(tr,x)
      {
       return tr.TEnter.call(null,x);
      },
      AnimateExit:function(tr,x)
      {
       return tr.TExit.call(null,x);
      },
      CanAnimateChange:function(tr)
      {
       return(tr.TFlags&1)!==0;
      },
      CanAnimateEnter:function(tr)
      {
       return(tr.TFlags&2)!==0;
      },
      CanAnimateExit:function(tr)
      {
       return(tr.TFlags&4)!==0;
      },
      Trivial:function()
      {
       return{
        TChange:function()
        {
         return function(y)
         {
          return An.Const(y);
         };
        },
        TEnter:function(t)
        {
         return An.Const(t);
        },
        TExit:function(t)
        {
         return An.Const(t);
        },
        TFlags:0
       };
      }
     }),
     Trans1:Runtime.Class({},{
      Change:function(ch,tr)
      {
       return{
        TChange:ch,
        TEnter:tr.TEnter,
        TExit:tr.TExit,
        TFlags:tr.TFlags|1
       };
      },
      Create:function(ch)
      {
       return{
        TChange:ch,
        TEnter:function(t)
        {
         return An.Const(t);
        },
        TExit:function(t)
        {
         return An.Const(t);
        },
        TFlags:1
       };
      },
      Enter:function(f,tr)
      {
       return{
        TChange:tr.TChange,
        TEnter:f,
        TExit:tr.TExit,
        TFlags:tr.TFlags|2
       };
      },
      Exit:function(f,tr)
      {
       return{
        TChange:tr.TChange,
        TEnter:tr.TEnter,
        TExit:f,
        TFlags:tr.TFlags|4
       };
      }
     }),
     Trie1:{
      AllSome:function(xs)
      {
       var e,r,ok,matchValue;
       e=Enumerator.Get(xs);
       r=ResizeArrayProxy.New2();
       ok=true;
       while(ok?e.MoveNext():false)
        {
         matchValue=e.get_Current();
         if(matchValue.$==1)
          {
           r.Add(matchValue.$0);
          }
         else
          {
           ok=false;
          }
        }
       return ok?{
        $:1,
        $0:r.ToArray()
       }:{
        $:0
       };
      },
      Empty:function()
      {
       return{
        $:1
       };
      },
      IsLeaf:function(t)
      {
       return t.$==2?true:false;
      },
      Leaf:function(v)
      {
       return{
        $:2,
        $0:v
       };
      },
      Look:function(key,trie)
      {
       var matchValue,ks,matchValue1;
       matchValue=[trie,key];
       if(matchValue[0].$==2)
        {
         return{
          $:0,
          $0:matchValue[0].$0,
          $1:key
         };
        }
       else
        {
         if(matchValue[0].$==0)
          {
           if(matchValue[1].$==1)
            {
             ks=matchValue[1].$1;
             matchValue1=MapModule.TryFind(matchValue[1].$0,matchValue[0].$0);
             return matchValue1.$==0?{
              $:1
             }:Trie1.Look(ks,matchValue1.$0);
            }
           else
            {
             return{
              $:1
             };
            }
          }
         else
          {
           return{
            $:1
           };
          }
        }
      },
      Lookup:function(trie,key)
      {
       return Trie1.Look(Seq.toList(key),trie);
      },
      Map:function(f,trie)
      {
       return Trie1.MapLoop(Runtime.New(T,{
        $:0
       }),f,trie);
      },
      MapLoop:function(loc,f,trie)
      {
       var x;
       if(trie.$==1)
        {
         return{
          $:1
         };
        }
       else
        {
         if(trie.$==2)
          {
           x=trie.$0;
           return{
            $:2,
            $0:(f(loc))(x)
           };
          }
         else
          {
           return Trie1.TrieBranch(MapModule.Map(function(k)
           {
            return function(v)
            {
             return Trie1.MapLoop(List.append(loc,List.ofArray([k])),f,v);
            };
           },trie.$0));
          }
        }
      },
      Mapi:function(f,trie)
      {
       var counter;
       counter=[0];
       return Trie1.Map(function(x)
       {
        var c;
        c=counter[0];
        counter[0]=c+1;
        return(f(c))(x);
       },trie);
      },
      Merge:function(ts)
      {
       var ts1,matchValue;
       ts1=Seq.toArray(ts);
       matchValue=Arrays.length(ts1);
       return matchValue===0?{
        $:1,
        $0:{
         $:1
        }
       }:matchValue===1?{
        $:1,
        $0:Arrays.get(ts1,0)
       }:Seq.exists(function(t)
       {
        return Trie1.IsLeaf(t);
       },ts1)?{
        $:0
       }:Option.map(function(xs)
       {
        return Trie1.TrieBranch(xs);
       },Trie1.MergeMaps(function(_arg00_)
       {
        return Trie1.Merge(_arg00_);
       },Seq.choose(function(_arg1)
       {
        return _arg1.$==0?{
         $:1,
         $0:_arg1.$0
        }:{
         $:0
        };
       },ts1)));
      },
      MergeMaps:function(merge,maps)
      {
       var x;
       x=Seq.collect(function(table)
       {
        return MapModule.ToSeq(table);
       },maps);
       return Option.map(function(elements)
       {
        return MapModule.OfArray(Seq.toArray(elements));
       },Trie1.AllSome(Seq.map(function(tupledArg)
       {
        var k;
        k=tupledArg[0];
        return Option.map(function(v)
        {
         return[k,v];
        },merge(tupledArg[1]));
       },MapModule.ToSeq(Seq.fold(function(s)
       {
        return function(tupledArg)
        {
         return Trie1.MultiAdd(tupledArg[0],tupledArg[1],s);
        };
       },FSharpMap.New1([]),x)))));
      },
      MultiAdd:function(key,value,map)
      {
       return map.Add(key,Runtime.New(T,{
        $:1,
        $0:value,
        $1:Trie1.MultiFind(key,map)
       }));
      },
      MultiFind:function(key,map)
      {
       return Operators.DefaultArg(MapModule.TryFind(key,map),Runtime.New(T,{
        $:0
       }));
      },
      Prefix:function(key,trie)
      {
       return Trie1.TrieBranch(FSharpMap.New1(List.ofArray([[key,trie]])));
      },
      ToArray:function(trie)
      {
       var all;
       all=[];
       Trie1.Map(function()
       {
        return function(v)
        {
         return JQueue.Add(v,all);
        };
       },trie);
       return JQueue.ToArray(all);
      },
      TrieBranch:function(xs)
      {
       return xs.get_IsEmpty()?{
        $:1
       }:{
        $:0,
        $0:xs
       };
      }
     },
     Var:Runtime.Class({
      Get:function()
      {
       return Var1.Get(this);
      },
      Set:function(v)
      {
       return Var1.Set(this,v);
      },
      Update:function(f)
      {
       return Var1.Update(this,f);
      },
      UpdateMaybe:function(f)
      {
       var matchValue;
       matchValue=f(Var1.Get(this));
       return matchValue.$==1?Var1.Set(this,matchValue.$0):null;
      },
      get_Id:function()
      {
       return"uinref"+Global.String(Var1.GetId(this));
      },
      get_View:function()
      {
       var _this=this;
       return{
        $:0,
        $0:function()
        {
         return Var1.Observe(_this);
        }
       };
      },
      get_View1:function()
      {
       return this.get_View();
      }
     },{
      Create:function(v)
      {
       return Runtime.New(Var,{
        Const:false,
        Current:v,
        Snap:Snap1.CreateWithValue(v),
        Id:Fresh.Int()
       });
      }
     }),
     Var1:Runtime.Class({},{
      Get:function(_var)
      {
       return _var.Current;
      },
      GetId:function(_var)
      {
       return _var.Id;
      },
      Lens:function(iref,get,update)
      {
       return RefImpl.New(iref,get,update);
      },
      Observe:function(_var)
      {
       return _var.Snap;
      },
      Set:function(_var,value)
      {
       if(_var.Const)
        {
         return null;
        }
       else
        {
         Snap1.MarkObsolete(_var.Snap);
         _var.Current=value;
         _var.Snap=Snap1.CreateWithValue(value);
         return;
        }
      },
      SetFinal:function(_var,value)
      {
       if(_var.Const)
        {
         return null;
        }
       else
        {
         _var.Const=true;
         _var.Current=value;
         _var.Snap=Snap1.CreateForever(value);
         return;
        }
      },
      Update:function(_var,fn)
      {
       return Var1.Set(_var,fn(Var1.Get(_var)));
      }
     }),
     View:Runtime.Class({},{
      Convert:function(conv,view)
      {
       return View.ConvertBy(function(x)
       {
        return x;
       },conv,view);
      },
      ConvertBy:function(key,conv,view)
      {
       var state;
       state=[Dictionary.New12()];
       return View.Map(function(xs)
       {
        var prevState,newState,result;
        prevState=state[0];
        newState=Dictionary.New12();
        result=Arrays.map(function(x)
        {
         var k,res;
         k=key(x);
         res=prevState.ContainsKey(k)?prevState.get_Item(k):conv(x);
         newState.set_Item(k,res);
         return res;
        },Seq.toArray(xs));
        state[0]=newState;
        return result;
       },view);
      },
      ConvertSeq:function(conv,view)
      {
       return View.ConvertSeqBy(function(x)
       {
        return x;
       },function()
       {
        return function(v)
        {
         return conv(v);
        };
       },view);
      },
      ConvertSeqBy:function(key,conv,view)
      {
       var state;
       state=[Dictionary.New12()];
       return View.Map(function(xs)
       {
        var prevState,newState,result;
        prevState=state[0];
        newState=Dictionary.New12();
        result=Arrays.map(function(x)
        {
         var k,node,n;
         k=key(x);
         if(prevState.ContainsKey(k))
          {
           n=prevState.get_Item(k);
           Var1.Set(n.NVar,x);
           node=n;
          }
         else
          {
           node=View.ConvertSeqNode(function(v)
           {
            return(conv(k))(v);
           },x);
          }
         newState.set_Item(k,node);
         return node.NValue;
        },Seq.toArray(xs));
        state[0]=newState;
        return result;
       },view);
      },
      ConvertSeqNode:function(conv,value)
      {
       var _var,view;
       _var=Var.Create(value);
       view=_var.get_View();
       return{
        NValue:conv(view),
        NVar:_var,
        NView:view
       };
      },
      CreateLazy:function(observe)
      {
       var cur;
       cur=[{
        $:0
       }];
       return{
        $:0,
        $0:function()
        {
         var matchValue,sn,sn1;
         matchValue=cur[0];
         if(matchValue.$==1)
          {
           if(!Snap1.IsObsolete(matchValue.$0))
            {
             return matchValue.$0;
            }
           else
            {
             sn=observe(null);
             cur[0]={
              $:1,
              $0:sn
             };
             return sn;
            }
          }
         else
          {
           sn1=observe(null);
           cur[0]={
            $:1,
            $0:sn1
           };
           return sn1;
          }
        }
       };
      },
      CreateLazy2:function(snapFn,_arg4,_arg3)
      {
       var o1,o2;
       o1=_arg4.$0;
       o2=_arg3.$0;
       return View.CreateLazy(function()
       {
        var s1,s2;
        s1=o1(null);
        s2=o2(null);
        return(snapFn(s1))(s2);
       });
      },
      Join:function(_arg8)
      {
       var observe;
       observe=_arg8.$0;
       return View.CreateLazy(function()
       {
        return Snap1.Bind(function(_arg2)
        {
         return _arg2.$0.call(null,null);
        },observe(null));
       });
      },
      Map:function(fn,_arg1)
      {
       var observe;
       observe=_arg1.$0;
       return View.CreateLazy(function()
       {
        return Snap1.Map(fn,observe(null));
       });
      },
      Map2:function(fn,v1,v2)
      {
       return View.CreateLazy2(function(_arg10_)
       {
        return function(_arg20_)
        {
         return Snap1.Map2(fn,_arg10_,_arg20_);
        };
       },v1,v2);
      },
      MapAsync:function(fn,_arg5)
      {
       var observe;
       observe=_arg5.$0;
       return View.CreateLazy(function()
       {
        return Snap1.MapAsync(fn,observe(null));
       });
      },
      MapCached:function(fn,_arg2)
      {
       var observe,vref;
       observe=_arg2.$0;
       vref=[{
        $:0
       }];
       return View.CreateLazy(function()
       {
        return Snap1.MapCached(vref,fn,observe(null));
       });
      },
      SnapshotOn:function(def,_arg7,_arg6)
      {
       var o1,o2,res,init;
       o1=_arg7.$0;
       o2=_arg6.$0;
       res=Snap1.CreateWithValue(def);
       init=[false];
       return View.CreateLazy(function()
       {
        var s1,s2;
        s1=o1(null);
        s2=o2(null);
        if(init[0])
         {
          return Snap1.SnapshotOn(s1,s2);
         }
        else
         {
          Snap1.When(Snap1.SnapshotOn(s1,s2),function()
          {
           return null;
          },function()
          {
           if(!init[0])
            {
             init[0]=true;
             return Snap1.MarkObsolete(res);
            }
           else
            {
             return null;
            }
          });
          return res;
         }
       });
      },
      UpdateWhile:function(def,v1,v2)
      {
       var value;
       value=[def];
       return View.Map2(function(pred)
       {
        return function(v)
        {
         if(pred)
          {
           value[0]=v;
          }
         return value[0];
        };
       },v1,v2);
      },
      get_Do:function()
      {
       return{
        $:0
       };
      }
     }),
     View1:Runtime.Class({},{
      Apply:function(fn,view)
      {
       return View.Map2(function(f)
       {
        return function(x)
        {
         return f(x);
        };
       },fn,view);
      },
      Bind:function(fn,view)
      {
       return View.Join(View.Map(fn,view));
      },
      Const:function(x)
      {
       var o;
       o=Snap1.CreateForever(x);
       return{
        $:0,
        $0:function()
        {
         return o;
        }
       };
      },
      Sink:function(act,_arg9)
      {
       var observe,loop;
       observe=_arg9.$0;
       loop=function()
       {
        return Snap1.When(observe(null),act,function()
        {
         return Async.Schedule(loop);
        });
       };
       return Async.Schedule(loop);
      }
     })
    }
   }
  }
 });
 Runtime.OnInit(function()
 {
  Concurrency=Runtime.Safe(Global.WebSharper.Concurrency);
  Array=Runtime.Safe(Global.Array);
  Seq=Runtime.Safe(Global.WebSharper.Seq);
  Arrays=Runtime.Safe(Global.WebSharper.Arrays);
  UI=Runtime.Safe(Global.WebSharper.UI);
  Next=Runtime.Safe(UI.Next);
  Abbrev=Runtime.Safe(Next.Abbrev);
  Fresh=Runtime.Safe(Abbrev.Fresh);
  Collections=Runtime.Safe(Global.WebSharper.Collections);
  HashSetProxy=Runtime.Safe(Collections.HashSetProxy);
  HashSet=Runtime.Safe(Abbrev.HashSet);
  JQueue=Runtime.Safe(Abbrev.JQueue);
  Slot1=Runtime.Safe(Abbrev.Slot1);
  Unchecked=Runtime.Safe(Global.WebSharper.Unchecked);
  An=Runtime.Safe(Next.An);
  AppendList1=Runtime.Safe(Next.AppendList1);
  Anims=Runtime.Safe(Next.Anims);
  requestAnimationFrame=Runtime.Safe(Global.requestAnimationFrame);
  Lazy=Runtime.Safe(Global.WebSharper.Lazy);
  Array1=Runtime.Safe(Next.Array);
  Trans=Runtime.Safe(Next.Trans);
  Option=Runtime.Safe(Global.WebSharper.Option);
  View=Runtime.Safe(Next.View);
  Client=Runtime.Safe(Next.Client);
  Attrs=Runtime.Safe(Client.Attrs);
  DomUtility=Runtime.Safe(Next.DomUtility);
  Attr=Runtime.Safe(Client.Attr);
  AttrProxy=Runtime.Safe(Client.AttrProxy);
  List=Runtime.Safe(Global.WebSharper.List);
  AnimatedAttrNode=Runtime.Safe(Client.AnimatedAttrNode);
  DynamicAttrNode=Runtime.Safe(Client.DynamicAttrNode);
  View1=Runtime.Safe(Next.View1);
  document=Runtime.Safe(Global.document);
  Doc=Runtime.Safe(Client.Doc);
  Elt=Runtime.Safe(Client.Elt);
  Seq1=Runtime.Safe(Global.Seq);
  Docs=Runtime.Safe(Client.Docs);
  String=Runtime.Safe(Next.String);
  CheckedInput=Runtime.Safe(Client.CheckedInput);
  Mailbox=Runtime.Safe(Abbrev.Mailbox);
  Operators=Runtime.Safe(Global.WebSharper.Operators);
  T=Runtime.Safe(List.T);
  jQuery=Runtime.Safe(Global.jQuery);
  NodeSet=Runtime.Safe(Docs.NodeSet);
  DocElemNode=Runtime.Safe(Client.DocElemNode);
  DomNodes=Runtime.Safe(Docs.DomNodes);
  Var1=Runtime.Safe(Next.Var1);
  RegExp=Runtime.Safe(Global.RegExp);
  Var=Runtime.Safe(Next.Var);
  Easing=Runtime.Safe(Next.Easing);
  Easings=Runtime.Safe(Next.Easings);
  FlowBuilder=Runtime.Safe(Next.FlowBuilder);
  Flow1=Runtime.Safe(Next.Flow1);
  Input=Runtime.Safe(Next.Input);
  DoubleInterpolation=Runtime.Safe(Next.DoubleInterpolation);
  Key=Runtime.Safe(Next.Key);
  ListModels=Runtime.Safe(Next.ListModels);
  RefImpl1=Runtime.Safe(Next.RefImpl1);
  ListModel=Runtime.Safe(Next.ListModel);
  Model1=Runtime.Safe(Next.Model1);
  Model=Runtime.Safe(Next.Model);
  Strings=Runtime.Safe(Global.WebSharper.Strings);
  encodeURIComponent=Runtime.Safe(Global.encodeURIComponent);
  decodeURIComponent=Runtime.Safe(Global.decodeURIComponent);
  Route=Runtime.Safe(Next.Route);
  Routing=Runtime.Safe(Next.Routing);
  Router1=Runtime.Safe(Next.Router1);
  Trie1=Runtime.Safe(Next.Trie1);
  Dictionary=Runtime.Safe(Collections.Dictionary);
  window=Runtime.Safe(Global.window);
  Snap1=Runtime.Safe(Next.Snap1);
  Async=Runtime.Safe(Abbrev.Async);
  Char=Runtime.Safe(Global.WebSharper.Char);
  Submitter=Runtime.Safe(Next.Submitter);
  Enumerator=Runtime.Safe(Global.WebSharper.Enumerator);
  ResizeArray=Runtime.Safe(Collections.ResizeArray);
  ResizeArrayProxy=Runtime.Safe(ResizeArray.ResizeArrayProxy);
  MapModule=Runtime.Safe(Collections.MapModule);
  FSharpMap=Runtime.Safe(Collections.FSharpMap);
  return RefImpl=Runtime.Safe(Next.RefImpl);
 });
 Runtime.OnLoad(function()
 {
  Runtime.Inherit(Elt,Doc);
  Input.MousePosSt1();
  Input.MouseBtnSt1();
  Input.KeyListenerState();
  Input.ActivateKeyListener();
  Input.ActivateButtonListener();
  Easings.CubicInOut();
  DomUtility.Doc();
  Attrs.EmptyAttr();
  Attr.ValidateForm();
  Fresh.counter();
  return;
 });
}());
