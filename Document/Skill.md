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
# 技能设计
## 技能打断
约定0 - 100 为保留打断类型，100以上由策划设计。
目前 打断类型 1 为 移动打断。
## 召唤物
技能召唤的召唤物都需要Unity View组件，需要移动的要增加Unity Rigid组件，需要物理碰撞的要增加Collider组件。
根据需要选择Layer类型，详细的Layer类型见物理章节。
## 物理
### 技能大量使用了仿真物理，主要为了实现
* 伤害判定 ：多用trigger触发
* 物理移动 ：多用Collider实现
### <span id="layer">Layer</span>
* Wall
* InvisibelWall
* Terrain
* Player
* Character
* TriggerBullet
* PhysicsBullet


# 命令说明
## 通用命令
### wait
等待指定的时间，毫秒为单位
```
wait(milliseconds);
```
### terminate
结束脚本。
```
terminate();
```
### log
输出日志。
```
log("hello world.");
```
### <span id="loopcommand">loop</span>
将一组指令循环执行指定次数
```
loop(10)
{
    log($$);
};
```
### <span id="loopcommand">looplist</span>
类似于[loop](#loopcommand), 迭代元素替换为list元素
```
looplist(@targets)
{
    log($$);
};
```
## 特定命令
### animation
播放动作
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
设置动作速度
```
animationspeed("skill_01_animation", 2);
```
### curvemove
变加速移动
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
    direction(direction_type, always_update_direction);
};
```
```
curvemove(true, 1, 0, 6, 6, 0, 0, 0, 1, 0, 0, 6, 0, 0, 0, 1, 0, -6, 6, 0, 0, 0)
{
    direction(0);
};
curvemove(true, 1, 0, 6, 6, 0, 0, 0, 1, 0, 0, 6, 0, 0, 0, 1, 0, -6, 6, 0, 0, 0)
{
    direction(2, true);
};
```
### circlemove
极坐标下的变加速移动
```
circlemove(start_distance, start_angle, [movetime, radius_speed, angle_speed, radius_accel, angle_accel]+);
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
### colliderdamage
物理检测伤害
```
colliderdamage(layer, remain_time, damage_interval, vector3(offsetx, offsety, offsetz), vector3(sizex, sizey, sizez))
{
    box(vector3(offsetx, offsety, offsetz), vector3(sizex, sizey, sizez));
    line(vector3(startposx, startposy, startposz), vector3(startposx, startposy, startposz), radius);
    statebuff("Default", default_buff);
    statebuff("Skill", skill_buff);
};
```
```
// box collider
colliderdamage("TriggerBullet", 1000, 100)
{
    box(vector3(0, 0, 0), vector3(1, 1, 1));
    statebuff("Default", 1001);
    statebuff("Skill", 1002);
};
// line collider
colliderdamage("TriggerBullet", 1000, 100)
{
    line(vector3(0, 0, 0), vector3(1, 1, 1), 0.1f);
    statebuff("Default", 1001);
    statebuff("Skill", 1002);
};
```
### changelayer
改变物理组件的layer，从而改变碰撞检关系。
[Layer列表](#layer)
```
changelayer(layer_name);
```
```
changelayer("PhysicsBullet");
```
### removecollider
移除检测伤害的物理组件
```
removecollider();
```

### movechild
移动挂接模型到指定节点
```
movechild("1_JianShi_w_01", "ef_backweapon01"); // 将子节点1_JianShi_w_01移动到挂点ef_backweapon01
```

### effect
特效命令
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
### findtarget
寻找指定目标
```
findtarget(vector3(offsetx, offsety, offsetz), radius)objid(@retval);
```
```
findtarget(vector3(0, 0, 0), 10)objid(@targetidlist);
looplist(@targetidlist)
{
    log($$);
};
```
### createcharacter
创建character
```
createcharacter(character_id, vector3(posX, posY, posZ), vector3(rotationX, rotationY, rotationZ))
{
    mainplayer();
    skill(skill_id);
}objid(@retval);
```
```
createcharacter(1, vector3(0, 0, 0), vector3(0, 90, 0)objid(@characterid);

createcharacter(1, vector3(0, 0, 1), vector3(0, 90, 0)
{
    skill(1001);
};
```
### visible
关闭渲染组件
```
visible(true_of_false);
```
```
visible(false);
```
### skill
强制指定目标释放技能
```
skill(enity_id, skill_id);
```
```
findtarget(vector3(0, 0, 0), 10)objid(@targetidlist);
looplist(@targetidlist)
{
    skill($$, 5);
};
```
### camp
设置目标阵营
保留阵营目前是 主角阵营：0， 怪物阵营：1.
```
camp(entity_id, camp_id);
```
```
findtarget(vector3(0, 0, 0), 10)objid(@targetidlist);
looplist(@targetidlist)
{
    camp($$, 5);
};
```

