(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,Testing,Pervasives,SubtestBuilder,Random,Sample,Runner,Concurrency,Math,Unchecked,List,TestBuilder,QUnit,TestCategoryBuilder,Arrays,NaN1,Infinity1,Seq,Operators,String,Runner1,RunnerControlBody,document;
 Runtime.Define(Global,{
  WebSharper:{
   Testing:{
    Pervasives:{
     Do:Runtime.Field(function()
     {
      return SubtestBuilder.New();
     }),
     PropertyWith:function(name,gen,f)
     {
      var _builder_;
      _builder_=Pervasives.Test(name);
      return _builder_.Run(_builder_.PropertyWithSample(_builder_.Yield(null),function()
      {
       return Sample.Make(gen,100);
      },function()
      {
       return f;
      }));
     },
     PropertyWithSample:function(name,set,f)
     {
      var _builder_;
      _builder_=Pervasives.Test(name);
      return _builder_.Run(_builder_.PropertyWithSample(_builder_.Yield(null),function()
      {
       return set;
      },function()
      {
       return f;
      }));
     },
     Runner:{
      AddTest:function(t,r,asserter)
      {
       var f,x;
       f=function(args)
       {
        (t(asserter))(args);
        return args;
       };
       x=r(asserter);
       return Runner.Map(f,x);
      },
      AddTestAsync:function(t,r,asserter)
      {
       var f,x;
       f=function(args)
       {
        return Concurrency.Delay(function()
        {
         return Concurrency.Bind((t(asserter))(args),function()
         {
          return Concurrency.Return(args);
         });
        });
       };
       x=r(asserter);
       return Runner.MapAsync(f,x);
      },
      Bind:function(f,x)
      {
       var _,args,args1;
       if(x.$==1)
        {
         args=x.$0;
         _={
          $:1,
          $0:Concurrency.Delay(function()
          {
           return Concurrency.Bind(args,function(_arg1)
           {
            return Runner.ToAsync(f(_arg1));
           });
          })
         };
        }
       else
        {
         args1=x.$0;
         _=f(args1);
        }
       return _;
      },
      Map:function(f,x)
      {
       var _,args,args1;
       if(x.$==1)
        {
         args=x.$0;
         _={
          $:1,
          $0:Concurrency.Delay(function()
          {
           return Concurrency.Bind(args,function(_arg1)
           {
            return Concurrency.Return(f(_arg1));
           });
          })
         };
        }
       else
        {
         args1=x.$0;
         _={
          $:0,
          $0:f(args1)
         };
        }
       return _;
      },
      MapAsync:function(f,x)
      {
       var _,args,args1;
       if(x.$==1)
        {
         args=x.$0;
         _={
          $:1,
          $0:Concurrency.Delay(function()
          {
           return Concurrency.Bind(args,function(_arg1)
           {
            return f(_arg1);
           });
          })
         };
        }
       else
        {
         args1=x.$0;
         _={
          $:1,
          $0:f(args1)
         };
        }
       return _;
      },
      ToAsync:function(x)
      {
       var _,args,args1;
       if(x.$==1)
        {
         args=x.$0;
         _=args;
        }
       else
        {
         args1=x.$0;
         _=Concurrency.Delay(function()
         {
          return Concurrency.Return(args1);
         });
        }
       return _;
      }
     },
     SubtestBuilder:Runtime.Class({
      ApproxEqual:function(r,actual,expected)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         var actual1,expected1;
         actual1=actual(args);
         expected1=expected(args);
         return asserter.push(Math.abs(actual1-expected1)<0.0001,actual1,expected1);
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      ApproxEqualAsync:function(r,actual,expected)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          var expected1;
          expected1=expected(args);
          return Concurrency.Bind(actual(args),function(_arg9)
          {
           return Concurrency.Return(asserter.push(Math.abs(_arg9-expected1)<0.0001,_arg9,expected1));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      ApproxEqualMsg:function(r,actual,expected,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         var actual1,expected1;
         actual1=actual(args);
         expected1=expected(args);
         return asserter.push(Math.abs(actual1-expected1)<0.0001,actual1,expected1,message);
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      ApproxEqualMsgAsync:function(r,actual,expected,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          var expected1;
          expected1=expected(args);
          return Concurrency.Bind(actual(args),function(_arg10)
          {
           return Concurrency.Return(asserter.push(Math.abs(_arg10-expected1)<0.0001,_arg10,expected1,message));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      Bind:function(a,f)
      {
       return function(asserter)
       {
        return{
         $:1,
         $0:Concurrency.Delay(function()
         {
          return Concurrency.Bind(a,function(_arg21)
          {
           var matchValue,_,b,b1;
           matchValue=(f(_arg21))(asserter);
           if(matchValue.$==1)
            {
             b=matchValue.$0;
             _=b;
            }
           else
            {
             b1=matchValue.$0;
             _=Concurrency.Return(b1);
            }
           return _;
          });
         })
        };
       };
      },
      DeepEqual:function(r,actual,expected)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return asserter.deepEqual(actual(args),expected(args));
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      DeepEqualAsync:function(r,actual,expected)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          var expected1;
          expected1=expected(args);
          return Concurrency.Bind(actual(args),function(_arg7)
          {
           return Concurrency.Return(asserter.deepEqual(_arg7,expected1));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      DeepEqualMsg:function(r,actual,expected,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return asserter.deepEqual(actual(args),expected(args),message);
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      DeepEqualMsgAsync:function(r,actual,expected,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          var expected1;
          expected1=expected(args);
          return Concurrency.Bind(actual(args),function(_arg8)
          {
           return Concurrency.Return(asserter.deepEqual(_arg8,expected1,message));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      Equal:function(r,actual,expected)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         var actual1,expected1;
         actual1=actual(args);
         expected1=expected(args);
         return asserter.push(Unchecked.Equals(actual1,expected1),actual1,expected1);
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      EqualAsync:function(r,actual,expected)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          var expected1;
          expected1=expected(args);
          return Concurrency.Bind(actual(args),function(_arg1)
          {
           return Concurrency.Return(asserter.push(Unchecked.Equals(_arg1,expected1),_arg1,expected1));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      EqualMsg:function(r,actual,expected,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         var actual1,expected1;
         actual1=actual(args);
         expected1=expected(args);
         return asserter.push(Unchecked.Equals(actual1,expected1),actual1,expected1,message);
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      EqualMsgAsync:function(r,actual,expected,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          var expected1;
          expected1=expected(args);
          return Concurrency.Bind(actual(args),function(_arg2)
          {
           return Concurrency.Return(asserter.push(Unchecked.Equals(_arg2,expected1),_arg2,expected1,message));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      Expect:function(r,assertionCount)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return asserter.expect(assertionCount(args));
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      For:function(sample,f)
      {
       return function(asserter)
       {
        var loop,_,_1,_2,l,e,r,f1,x,_3;
        loop=[];
        _=sample.get_Data();
        _1={
         $:0,
         $0:undefined
        };
        loop[2]=_;
        loop[1]=_1;
        loop[0]=1;
        while(loop[0])
         {
          if(loop[2].$==1)
           {
            l=loop[2].$1;
            e=loop[2].$0;
            r=f(e);
            f1=function()
            {
             return r(asserter);
            };
            x=loop[1];
            _3=Runner.Bind(f1,x);
            loop[2]=l;
            loop[1]=_3;
            _2=void(loop[0]=1);
           }
          else
           {
            loop[0]=0;
            _2=void(loop[1]=loop[1]);
           }
         }
        return loop[1];
       };
      },
      For1:function(gen,f)
      {
       return this.For(Sample.New(gen),f);
      },
      For2:function(r,y)
      {
       return function(asserter)
       {
        var matchValue,_,a,a1;
        matchValue=r(asserter);
        if(matchValue.$==1)
         {
          a=matchValue.$0;
          _={
           $:1,
           $0:Concurrency.Delay(function()
           {
            return Concurrency.Bind(a,function(_arg22)
            {
             var matchValue1,_1,b,b1;
             matchValue1=(y(_arg22))(asserter);
             if(matchValue1.$==1)
              {
               b=matchValue1.$0;
               _1=b;
              }
             else
              {
               b1=matchValue1.$0;
               _1=Concurrency.Return(b1);
              }
             return _1;
            });
           })
          };
         }
        else
         {
          a1=matchValue.$0;
          _=(y(a1))(asserter);
         }
        return _;
       };
      },
      ForEach:function(r,src,attempt)
      {
       return function(asserter)
       {
        var loop,f2,x1;
        loop=function(attempt1,acc,src1)
        {
         var _,l,e,r1,f;
         if(src1.$==1)
          {
           l=src1.$1;
           e=src1.$0;
           r1=attempt1(e);
           f=function(args)
           {
            var f1,x;
            f1=function()
            {
             return args;
            };
            x=r1(asserter);
            return Runner.Map(f1,x);
           };
           _=loop(attempt1,Runner.Bind(f,acc),l);
          }
         else
          {
           _=acc;
          }
         return _;
        };
        f2=function(args)
        {
         return loop(attempt(args),{
          $:0,
          $0:args
         },List.ofSeq(src(args)));
        };
        x1=r(asserter);
        return Runner.Bind(f2,x1);
       };
      },
      IsFalse:function(r,value)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return asserter.ok(!value(args));
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      IsFalseAsync:function(r,value)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          return Concurrency.Bind(value(args),function(_arg15)
          {
           return Concurrency.Return(asserter.ok(!_arg15));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      IsFalseAsync1:function(r,value,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          return Concurrency.Bind(value(args),function(_arg16)
          {
           return Concurrency.Return(asserter.ok(!_arg16,message));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      IsFalseMsg:function(r,value,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return asserter.ok(!value(args),message);
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      IsTrue:function(r,value)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return asserter.ok(value(args));
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      IsTrueAsync:function(r,value)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          return Concurrency.Bind(value(args),function(_arg13)
          {
           return Concurrency.Return(asserter.ok(_arg13));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      IsTrueMsg:function(r,value,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return asserter.ok(value(args),message);
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      IsTrueMsgAsync:function(r,value,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          return Concurrency.Bind(value(args),function(_arg14)
          {
           return Concurrency.Return(asserter.ok(_arg14,message));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      JsEqual:function(r,actual,expected)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return asserter.equal(actual(args),expected(args));
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      JsEqualAsync:function(r,actual,expected)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          var expected1;
          expected1=expected(args);
          return Concurrency.Bind(actual(args),function(_arg5)
          {
           return Concurrency.Return(asserter.equal(_arg5,expected1));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      JsEqualMsg:function(r,actual,expected,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return asserter.equal(actual(args),expected(args),message);
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      JsEqualMsgAsync:function(r,actual,expected,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          var expected1;
          expected1=expected(args);
          return Concurrency.Bind(actual(args),function(_arg6)
          {
           return Concurrency.Return(asserter.equal(_arg6,expected1,message));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      NotApproxEqual:function(r,actual,expected)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         var actual1,expected1;
         actual1=actual(args);
         expected1=expected(args);
         return asserter.push(Math.abs(actual1-expected1)>0.0001,actual1,expected1);
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      NotApproxEqualAsync:function(r,actual,expected)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          var expected1;
          expected1=expected(args);
          return Concurrency.Bind(actual(args),function(_arg11)
          {
           return Concurrency.Return(asserter.push(Math.abs(_arg11-expected1)>0.0001,_arg11,expected1));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      NotApproxEqualMsg:function(r,actual,expected,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         var actual1,expected1;
         actual1=actual(args);
         expected1=expected(args);
         return asserter.push(Math.abs(actual1-expected1)>0.0001,actual1,expected1,message);
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      NotApproxEqualMsgAsync:function(r,actual,expected,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          var expected1;
          expected1=expected(args);
          return Concurrency.Bind(actual(args),function(_arg12)
          {
           return Concurrency.Return(asserter.push(Math.abs(_arg12-expected1)>0.0001,_arg12,expected1,message));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      NotEqual:function(r,actual,expected)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         var actual1,expected1;
         actual1=actual(args);
         expected1=expected(args);
         return asserter.push(!Unchecked.Equals(actual1,expected1),actual1,expected1);
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      NotEqualAsync:function(r,actual,expected)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          var expected1;
          expected1=expected(args);
          return Concurrency.Bind(actual(args),function(_arg3)
          {
           return Concurrency.Return(asserter.push(!Unchecked.Equals(_arg3,expected1),_arg3,expected1));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      NotEqualMsg:function(r,actual,expected,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         var actual1,expected1;
         actual1=actual(args);
         expected1=expected(args);
         return asserter.push(!Unchecked.Equals(actual1,expected1),actual1,expected1,message);
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      NotEqualMsgAsync:function(r,actual,expected,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         return Concurrency.Delay(function()
         {
          var expected1;
          expected1=expected(args);
          return Concurrency.Bind(actual(args),function(_arg4)
          {
           return Concurrency.Return(asserter.push(!Unchecked.Equals(_arg4,expected1),_arg4,expected1,message));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      PropertyWithSample:function(r,sample,attempt)
      {
       return function(asserter)
       {
        var loop,f2,x1;
        loop=function(attempt1,acc,src)
        {
         var _,l,e,r1,f;
         if(src.$==1)
          {
           l=src.$1;
           e=src.$0;
           r1=attempt1(e);
           f=function(args)
           {
            var f1,x;
            f1=function()
            {
             return args;
            };
            x=r1(asserter);
            return Runner.Map(f1,x);
           };
           _=loop(attempt1,Runner.Bind(f,acc),l);
          }
         else
          {
           _=acc;
          }
         return _;
        };
        f2=function(args)
        {
         var sample1;
         sample1=sample(args);
         return loop(attempt(args),{
          $:0,
          $0:args
         },sample1.get_Data());
        };
        x1=r(asserter);
        return Runner.Bind(f2,x1);
       };
      },
      Raises:function(r,value)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         var _,value1,matchValue;
         try
         {
          value1=value(args);
          _=asserter.ok(false,"Expected raised exception");
         }
         catch(matchValue)
         {
          _=asserter.ok(true);
         }
         return _;
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      RaisesAsync:function(r,value)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         var value1;
         value1=value(args);
         return Concurrency.Delay(function()
         {
          return Concurrency.TryWith(Concurrency.Delay(function()
          {
           return Concurrency.Bind(value1,function()
           {
            return Concurrency.Return(asserter.ok(false,"Expected raised exception"));
           });
          }),function()
          {
           return Concurrency.Return(asserter.ok(true));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      RaisesMsg:function(r,value,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         var _,value1,matchValue;
         try
         {
          value1=value(args);
          _=asserter.ok(false,message);
         }
         catch(matchValue)
         {
          _=asserter.ok(true,message);
         }
         return _;
        };
       };
       return function(asserter)
       {
        return Runner.AddTest(t,r,asserter);
       };
      },
      RaisesMsgAsync:function(r,value,message)
      {
       var t;
       t=function(asserter)
       {
        return function(args)
        {
         var value1;
         value1=value(args);
         return Concurrency.Delay(function()
         {
          return Concurrency.TryWith(Concurrency.Delay(function()
          {
           return Concurrency.Bind(value1,function()
           {
            return Concurrency.Return(asserter.ok(false,message));
           });
          }),function()
          {
           return Concurrency.Return(asserter.ok(true,message));
          });
         });
        };
       };
       return function(asserter)
       {
        return Runner.AddTestAsync(t,r,asserter);
       };
      },
      Return:function(x)
      {
       return function()
       {
        return{
         $:0,
         $0:x
        };
       };
      },
      RunSubtest:function(r,subtest)
      {
       return function(asserter)
       {
        var f,x;
        f=function(a)
        {
         return(subtest(a))(asserter);
        };
        x=r(asserter);
        return Runner.Bind(f,x);
       };
      },
      Yield:function(x)
      {
       return function()
       {
        return{
         $:0,
         $0:x
        };
       };
      },
      Zero:function()
      {
       return function()
       {
        return{
         $:0,
         $0:undefined
        };
       };
      }
     },{
      New:function()
      {
       return Runtime.New(this,{});
      }
     }),
     Test:function(name)
     {
      return TestBuilder.New(name);
     },
     TestBuilder:Runtime.Class({
      Run:function(e)
      {
       return QUnit.test(this.name,function(asserter)
       {
        var _,matchValue,_1,asy,done,arg00,e1;
        try
        {
         matchValue=e(asserter);
         if(matchValue.$==1)
          {
           asy=matchValue.$0;
           done=asserter.async();
           arg00=Concurrency.Delay(function()
           {
            return Concurrency.TryFinally(Concurrency.Delay(function()
            {
             return Concurrency.TryWith(Concurrency.Delay(function()
             {
              return Concurrency.Bind(asy,function()
              {
               return Concurrency.Return(null);
              });
             }),function(_arg2)
             {
              return Concurrency.Return(asserter.equal(_arg2,null,"Test threw an unexpected asynchronous exception"));
             });
            }),function()
            {
             return done(null);
            });
           });
           _1=Concurrency.Start(arg00,{
            $:0
           });
          }
         else
          {
           _1=null;
          }
         _=_1;
        }
        catch(e1)
        {
         _=asserter.equal(e1,null,"Test threw an unexpected synchronous exception");
        }
        return _;
       });
      }
     },{
      New:function(name)
      {
       var r;
       r=Runtime.New(this,SubtestBuilder.New());
       r.name=name;
       return r;
      }
     }),
     TestCategory:function(name)
     {
      return TestCategoryBuilder.New(name);
     },
     TestCategoryBuilder:Runtime.Class({},{
      New:function(name)
      {
       var r;
       r=Runtime.New(this,{});
       r.name=name;
       return r;
      }
     })
    },
    Random:{
     Anything:Runtime.Field(function()
     {
      return Random.MixManyWithoutBases(Random.allTypes());
     }),
     ArrayOf:function(generator)
     {
      return{
       Base:[[]],
       Next:function()
       {
        var len;
        len=Random.Natural().Next.call(null,null)%100;
        return Arrays.init(len,function()
        {
         return generator.Next.call(null,null);
        });
       }
      };
     },
     Boolean:Runtime.Field(function()
     {
      return{
       Base:[true,false],
       Next:function()
       {
        return Random.StandardUniform().Next.call(null,null)>0.5;
       }
      };
     }),
     Choose:function(gens,f)
     {
      var f1,gen,gengen;
      f1=function(i)
      {
       return Arrays.get(gens,i);
      };
      gen=Random.Within(0,Arrays.length(gens)-1);
      gengen=Random.Map(f1,gen);
      return{
       Base:[],
       Next:function()
       {
        var gen1;
        gen1=gengen.Next.call(null,null);
        return f(gen1).Next.call(null,null);
       }
      };
     },
     Const:function(x)
     {
      return{
       Base:[x],
       Next:function()
       {
        return x;
       }
      };
     },
     Exponential:function(lambda)
     {
      return{
       Base:[],
       Next:function()
       {
        var p;
        p=Random.StandardUniform().Next.call(null,null);
        return-Math.log(1-p)/lambda;
       }
      };
     },
     Float:Runtime.Field(function()
     {
      return{
       Base:[0],
       Next:function()
       {
        var sign;
        sign=Random.Boolean().Next.call(null,null)?1:-1;
        return sign*Random.Exponential(0.1).Next.call(null,null);
       }
      };
     }),
     FloatExhaustive:Runtime.Field(function()
     {
      return{
       Base:[0,NaN1,Infinity1,-Infinity1],
       Next:function()
       {
        return Random.Float().Next.call(null,null);
       }
      };
     }),
     FloatWithin:function(low,hi)
     {
      return{
       Base:[low,hi],
       Next:function()
       {
        return low+(hi-low)*Math.random();
       }
      };
     },
     Implies:function(a,b)
     {
      return!a?true:b;
     },
     Imply:function(a,b)
     {
      return Random.Implies(a,b);
     },
     Int:Runtime.Field(function()
     {
      return{
       Base:[0,1,-1],
       Next:function()
       {
        return Math.round(Random.Float().Next.call(null,null))<<0;
       }
      };
     }),
     ListOf:function(generator)
     {
      var f,gen;
      f=function(array)
      {
       return List.ofArray(array);
      };
      gen=Random.ArrayOf(generator);
      return Random.Map(f,gen);
     },
     Map:function(f,gen)
     {
      var f1;
      f1=gen.Next;
      return{
       Base:Arrays.map(f,gen.Base),
       Next:function(x)
       {
        return f(f1(x));
       }
      };
     },
     Mix:function(a,b)
     {
      var left;
      left=[false];
      return{
       Base:a.Base.concat(b.Base),
       Next:function()
       {
        left[0]=!left[0];
        return left[0]?a.Next.call(null,null):b.Next.call(null,null);
       }
      };
     },
     MixMany:function(gs)
     {
      var i;
      i=[0];
      return{
       Base:Arrays.concat(Seq.toArray(Seq.delay(function()
       {
        return Seq.map(function(g)
        {
         return g.Base;
        },gs);
       }))),
       Next:function()
       {
        i[0]=(i[0]+1)%Arrays.length(gs);
        return Arrays.get(gs,i[0]).Next.call(null,null);
       }
      };
     },
     MixManyWithoutBases:function(gs)
     {
      var i;
      i=[0];
      return{
       Base:[],
       Next:function()
       {
        i[0]=(i[0]+1)%Arrays.length(gs);
        return Arrays.get(gs,i[0]).Next.call(null,null);
       }
      };
     },
     Natural:Runtime.Field(function()
     {
      var g;
      g=Random.Int().Next;
      return{
       Base:[0,1],
       Next:function(x)
       {
        var value;
        value=g(x);
        return Math.abs(value);
       }
      };
     }),
     OneOf:function(seeds)
     {
      var index;
      index=Random.Within(1,Arrays.length(seeds));
      return{
       Base:seeds,
       Next:function()
       {
        return Arrays.get(seeds,index.Next.call(null,null)-1);
       }
      };
     },
     OptionOf:function(generator)
     {
      return Random.Mix(Random.Const({
       $:0
      }),Random.Map(function(arg0)
      {
       return{
        $:1,
        $0:arg0
       };
      },generator));
     },
     Sample:Runtime.Class({
      get_Data:function()
      {
       return this.data;
      }
     },{
      Make:function(generator,count)
      {
       return Sample.New1(generator,count);
      },
      New:function(generator)
      {
       return Runtime.New(this,Sample.New1(generator,100));
      },
      New1:function(generator,count)
      {
       var data;
       data=Seq.toList(Seq.delay(function()
       {
        return Seq.append(Seq.map(function(i)
        {
         return Arrays.get(generator.Base,i);
        },Operators.range(0,Arrays.length(generator.Base)-1)),Seq.delay(function()
        {
         return Seq.map(function()
         {
          return generator.Next.call(null,null);
         },Operators.range(1,count));
        }));
       }));
       return Runtime.New(this,Sample.New4(data));
      },
      New4:function(data)
      {
       var r;
       r=Runtime.New(this,{});
       r.data=data;
       return r;
      }
     }),
     StandardUniform:Runtime.Field(function()
     {
      return{
       Base:[],
       Next:function()
       {
        return Math.random();
       }
      };
     }),
     String:Runtime.Field(function()
     {
      return{
       Base:[""],
       Next:function()
       {
        var len,cs;
        len=Random.Natural().Next.call(null,null)%100;
        cs=Arrays.init(len,function()
        {
         return Random.Int().Next.call(null,null)%256;
        });
        return String.fromCharCode.apply(undefined,cs);
       }
      };
     }),
     StringExhaustive:Runtime.Field(function()
     {
      return{
       Base:[null,""],
       Next:Random.String().Next
      };
     }),
     Tuple2Of:function(a,b)
     {
      return{
       Base:Seq.toArray(Seq.delay(function()
       {
        return Seq.collect(function(x)
        {
         return Seq.map(function(y)
         {
          return[x,y];
         },b.Base);
        },a.Base);
       })),
       Next:function()
       {
        return[a.Next.call(null,null),b.Next.call(null,null)];
       }
      };
     },
     Tuple3Of:function(a,b,c)
     {
      return{
       Base:Seq.toArray(Seq.delay(function()
       {
        return Seq.collect(function(x)
        {
         return Seq.collect(function(y)
         {
          return Seq.map(function(z)
          {
           return[x,y,z];
          },c.Base);
         },b.Base);
        },a.Base);
       })),
       Next:function()
       {
        return[a.Next.call(null,null),b.Next.call(null,null),c.Next.call(null,null)];
       }
      };
     },
     Within:function(low,hi)
     {
      return{
       Base:[low,hi],
       Next:function()
       {
        return Random.Natural().Next.call(null,null)%(hi-low)+low;
       }
      };
     },
     allTypes:Runtime.Field(function()
     {
      var _bases_273_1,_compose_280_3,composed;
      _bases_273_1=[Random.Int(),Random.Float(),Random.Boolean(),Random.String()];
      _compose_280_3=function(gs)
      {
       return Seq.toArray(Seq.delay(function()
       {
        return Seq.collect(function(g)
        {
         return Seq.append(Seq.collect(function(h)
         {
          return Seq.append([Random.Tuple2Of(g,h)],Seq.delay(function()
          {
           return Seq.map(function(i)
           {
            return Random.Tuple3Of(g,h,i);
           },gs);
          }));
         },gs),Seq.delay(function()
         {
          return Seq.append([Random.ListOf(g)],Seq.delay(function()
          {
           return[Random.ArrayOf(g)];
          }));
         }));
        },gs);
       }));
      };
      composed=_compose_280_3(_bases_273_1);
      return _bases_273_1.concat(composed);
     })
    },
    Runner:{
     RunnerControl:Runtime.Class({
      get_Body:function()
      {
       return RunnerControlBody.New();
      }
     }),
     RunnerControlBody:Runtime.Class({
      ReplaceInDom:function(e)
      {
       var fixture,qunit,parent,value,value1;
       fixture=document.createElement("div");
       fixture.setAttribute("id","qunit-fixture");
       qunit=document.createElement("div");
       qunit.setAttribute("id","qunit");
       parent=e.parentNode;
       value=parent.replaceChild(fixture,e);
       value1=parent.insertBefore(qunit,fixture);
       return;
      }
     },{
      New:function()
      {
       return Runtime.New(this,{});
      }
     })
    }
   }
  }
 });
 Runtime.OnInit(function()
 {
  Testing=Runtime.Safe(Global.WebSharper.Testing);
  Pervasives=Runtime.Safe(Testing.Pervasives);
  SubtestBuilder=Runtime.Safe(Pervasives.SubtestBuilder);
  Random=Runtime.Safe(Testing.Random);
  Sample=Runtime.Safe(Random.Sample);
  Runner=Runtime.Safe(Pervasives.Runner);
  Concurrency=Runtime.Safe(Global.WebSharper.Concurrency);
  Math=Runtime.Safe(Global.Math);
  Unchecked=Runtime.Safe(Global.WebSharper.Unchecked);
  List=Runtime.Safe(Global.WebSharper.List);
  TestBuilder=Runtime.Safe(Pervasives.TestBuilder);
  QUnit=Runtime.Safe(Global.QUnit);
  TestCategoryBuilder=Runtime.Safe(Pervasives.TestCategoryBuilder);
  Arrays=Runtime.Safe(Global.WebSharper.Arrays);
  NaN1=Runtime.Safe(Global.NaN);
  Infinity1=Runtime.Safe(Global.Infinity);
  Seq=Runtime.Safe(Global.WebSharper.Seq);
  Operators=Runtime.Safe(Global.WebSharper.Operators);
  String=Runtime.Safe(Global.String);
  Runner1=Runtime.Safe(Testing.Runner);
  RunnerControlBody=Runtime.Safe(Runner1.RunnerControlBody);
  return document=Runtime.Safe(Global.document);
 });
 Runtime.OnLoad(function()
 {
  Runtime.Inherit(TestBuilder,SubtestBuilder);
  Random.allTypes();
  Random.StringExhaustive();
  Random.String();
  Random.StandardUniform();
  Random.Natural();
  Random.Int();
  Random.FloatExhaustive();
  Random.Float();
  Random.Boolean();
  Random.Anything();
  Pervasives.Do();
  return;
 });
}());
