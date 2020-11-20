using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenUIManager_Chinese : ScreenUIManager
{
    public override void SetExplainingCourseTexts(int courseNumber) {
        difficultyText.gameObject.SetActive(true);
        proceedButton.gameObject.SetActive(true);
        courseText.gameObject.SetActive(true);
        ScreenUIManager.courseNumber = courseNumber;

        courseText.text = "航线" + courseNumber.ToString();
        if (1 <= courseNumber && courseNumber <= 3) {
            explanationText.text = "你能找到出口吗？";
            starText.text = "★";
        }
        else if (4 <= courseNumber && courseNumber <= 6) {
            explanationText.text = "走楼梯。";
            starText.text = "★";
        }
        else if (7 <= courseNumber && courseNumber <= 11) {
            explanationText.text = "桥会开路。";
            starText.text = "★★";
        }
        else if (courseNumber == 12) {
            explanationText.text = "墙会挡住你的路。";
            starText.text = "★★";
        }
        else if (courseNumber == 13) {
            explanationText.text = "妖怪会出现。";
            starText.text = "★";
        }
        else if (courseNumber == 14) {
            explanationText.text = "蓝色食人魔正在巡逻。";
            starText.text = "★★";
        }
        else if (courseNumber == 15) {
            explanationText.text = "墙外有一个绿色的食人魔。";
            starText.text = "★★";
        }
        else if (courseNumber == 16) {
            explanationText.text = "绿色的食人魔正在快乐地散步。";
            starText.text = "★★★";
        }
        else if (courseNumber == 17) {
            explanationText.text = "让红色食人魔离开你的路。";
            starText.text = "★★★";
        }
        else if (courseNumber == 18) {
            explanationText.text = "阻止绿色食人魔前进。";
            starText.text = "★★★★";
        }
        else if (courseNumber == 19) {
            explanationText.text = "来吧，绿色食人魔会欢迎你。";
            starText.text = "★★★★";
        }
        else if (courseNumber == 20) {
            explanationText.text = "让我们加入红色食人魔的队伍吧。";
            starText.text = "★★";
        }
        else if (courseNumber == 21) {
            explanationText.text = "红色食人魔不会让你通过逃生出口。";
            starText.text = "★★★★";
        }
        else if (courseNumber == 22) {
            explanationText.text = "很多墙体都被砌起来了。";
            starText.text = "★★";
        }
        else if (courseNumber == 23) {
            explanationText.text = "把红色食人魔引到你身边。";
            starText.text = "★★★";
        }
        else if (courseNumber == 24) {
            explanationText.text = "对不起，我在后面。";
            starText.text = "★★";
        }
        else if (courseNumber == 25) {
            explanationText.text = "只有一个守卫，蓝色食人魔。";
            starText.text = "★★";
        }
        else if (courseNumber == 26) {
            explanationText.text = "守卫是四个蓝色食人魔。";
            starText.text = "★★★";
        }
        else if (courseNumber == 27) {
            explanationText.text = "你可以看到白色食人魔后面有一个逃生出口。";
            starText.text = "★★★";
        }
        else if (courseNumber == 28) {
            explanationText.text = "蓝色食人魔正在配合。";
            starText.text = "★★★";
        }
        else if (courseNumber == 29) {
            explanationText.text = "白色食人魔挡住了你的路。\n绿色食人魔追赶你。\n蓝色食人魔守住了逃跑的路线。";
            starText.text = "★★★";
        }
        else if (courseNumber == 30) {
            explanationText.text = "你都带了吗？";
            starText.text = "★★";
        }
        else if (courseNumber == 31) {
            explanationText.text = "这是一场与绿色食人魔一对一的战斗。";
            starText.text = "★★★★";
        }
        else if (courseNumber == 32) {
            explanationText.text = "这条路是有限的。";
            starText.text = "★★★";
        }
        else if (courseNumber == 33) {
            explanationText.text = "快点，转身。";
            starText.text = "★★★★";
        }
        else if (courseNumber == 34) {
            explanationText.text = "蓝色食人魔正在和对方联手。";
            starText.text = "★★★★";
        }
        else if (courseNumber == 35) {
            explanationText.text = "这条路是有限的。";
            starText.text = "★★★★";
        }
        else if (courseNumber == 36) {
            explanationText.text = "绿色食人魔是受人崇拜的。";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 37) {
            explanationText.text = "你能打破绿色食人魔之间的协调吗？";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 38) {
            explanationText.text = "从蓝色食人魔的缺口处走桥。";
            starText.text = "★★★★";
        }
        else if (courseNumber == 39) {
            explanationText.text = "红色食人魔冲向你。";
            starText.text = "★★★★";
        }
        else if (courseNumber == 40) {
            explanationText.text = "迅速行动起来。";
            starText.text = "★★★★";
        }
        else if (courseNumber == 41) {
            explanationText.text = "城墙上有红色食人魔守卫。";
            starText.text = "★★★";
        }
        else if (courseNumber == 42) {
            explanationText.text = "绿色和红色的食人魔欢迎你的访问。";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 43) {
            explanationText.text = "你怎么逃？";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 44) {
            explanationText.text = "与蓝色食人魔一起呼吸。";
            starText.text = "★★★";
        }
        else if (courseNumber == 45) {
            explanationText.text = "让我们和蓝色食人魔一起去转转吧。";
            starText.text = "★★★★";
        }
        else if (courseNumber == 46) {
            explanationText.text = "就看你怎么玩蓝色食人魔了。";
            starText.text = "★★★★";
        }
        else if (courseNumber == 47) {
            explanationText.text = "看看蓝绿食人魔的呼吸。";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 48) {
            explanationText.text = "你能在红色食人魔来之前逃走吗？不，你不能。";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 49) {
            explanationText.text = "食人魔们都挤在一起。";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 50) {
            explanationText.text = "蓝色食人魔正在排队。";
            starText.text = "★★★★★";
        }
        else {
            courseText.text = "";
            explanationText.text = "";
            starText.text = "";
        }
    }
}
