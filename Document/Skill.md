# 简介

我们的技能配置采用的是自己设计的DSL语言。DSL语言是在工作中逐渐演化出来的一门交互式脚本语言。语法形式上类似于c语言。有c++和c#的paser和runtime。DSL语言的定位是介于lua，python脚本语言和传统txt，xml配置文件之间，通过提供编程语言的超小子集和强大的可读性配置在灵活性和易用性之间达到平衡。

# 语法元素

DSL语言的语句都是以分号(;)结尾，这可能是最容易写错的地方，在实际使用中一定要注意。DSL语言在语法层面由语句构成，语句由函数级联而成，最后加一个分号鞭尸语句结束。

语句 ::= { 函数 }；

函数 ::= 函数调用 { 语句列表 }

函数调用 ::= 函数名{ 函数参数表 }

函数调用 ::= 函数调用{ 函数参数表 }

DSL语言支持三种形式的注释：
```
/**/

//

#
```

DSL里的字符串可以用单引号或者双引号括起来。不带空格的字符串也可以省略括号。目前 true/false也被解释为字符串，可以认为DSL语言只有字符串和浮点数两种类型。

# 技能配置
```
skill(1)
{
	onmessage("start")
	{
		log("skill start");
		movechild("1_JianShi_w_01", "ef_rightweapon01");
		animation("zhankuang_julitiaokong_01");
		wait(33);
		animationspeed("zhankuang_julitiaokong_01", 2.5);
		wait(167);
		animationspeed("zhankuang_julitiaokong_01", 1.5);
		wait(66);
		animationspeed("zhankuang_julitiaokong_01", 2);
		wait(67);
		animationspeed("zhankuang_julitiaokong_01", 1.5);
		wait(133);
		animationspeed("zhankuang_julitiaokong_01", 0.125);
		wait(267);
		animationspeed("zhankuang_julitiaokong_01", 0.5);
		wait(167);
		setanimspeed("zhankuang_julitiaokong_01", 1);
		wait(700);
		animation("zhankuang_julitiaokong_02");
		areadamage(vector3(0, 0, 0), 3, 0);
		wait(1000);
		movechild("1_JianShi_w_01", "ef_backweapon01");
		terminate();
	};
};
```

# 命令说明
## 通用命令
### wait
```
wait(milliseconds);
```
等待指定的时间，毫秒为单位
### terminate
```
terminate();
```
结束技能。
### log
```
log("hello world.");
```
输出日志。
## 特定命令
### animation
```
animation("skill_01_animation");
```
```
animation("skill_01_animation")
{
    speed(1.0f);
    playmode(1, 1000); 0 = Play; 1 = CrossFade
    blendmode(0);  // 0 = Blend; 1 = Additive
    wrapmode(0);   // 0 = Default; 1 = Once; 2 = loop; 3 = PingPong; 4 = ClampForever
};
```
### animationspeed
```
animationspeed("skill_01_animation", 2);
```
### curvemove
```
enum direction_type
{
    target = 0,
    sender_target = 1,
    sender = 2,
    target_sender = 3,
    sender_opposite = 4,
}


curvemove(is_lock_rotate, [movetime, speedx, speedy, speedz, accelx, accely, accelz]+)
{
    direction(direction_type);
};
```
```
curvemove(true, 1, 0, 6, 6, 0, 0, 0, 1, 0, 0, 6, 0, 0, 0, 1, 0, -6, 6, 0, 0, 0)
{
    direction(0);
};
```
### areadamage
```
areadamage(vector3(offsetx, offsety, offsetz), radius)
{
    statebuff("Default", default_buff);
    statebuff("Skill", skill_buff);
}
```
```
areadamage(vector3(0, 0, 0), 3)
{
    statebuff("Default", 1001);
}
```

### movechild
```
movechild("1_JianShi_w_01", "ef_backweapon01"); // 将子节点1_JianShi_w_01移动到挂点ef_backweapon01
```

### effect
```
effect(res_path, delete_time, attach_bone, is_attach);
effect(res_path, delete_time, attach_bone, is_attach)
{
    transform(vector3(offsetx, offsety, offsetz), vector3(rotationx, rotationy, rotationz));
};

```
```
effect("Monster_FX/Campaign_1/6_Npc_Private_Attack_01", 3000, "", false);
effect("Monster_FX/Campaign_1/6_Npc_Private_Attack_01", 3000, "bone_root", ture)
{
  transform(vector3(1, 1, 1));  
};

```
