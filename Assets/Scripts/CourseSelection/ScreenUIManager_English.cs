using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenUIManager_English : ScreenUIManager
{
    public override void SetExplainingCourseTexts(int courseNumber) {
        difficultyText.gameObject.SetActive(true);
        proceedButton.gameObject.SetActive(true);
        courseText.gameObject.SetActive(true);
        ScreenUIManager.courseNumber = courseNumber;

        courseText.text = "Course" + courseNumber.ToString();
        if (1 <= courseNumber && courseNumber <= 3) {
            explanationText.text = "Can you find the exit?";
            starText.text = "★";
        }
        else if (4 <= courseNumber && courseNumber <= 6) {
            explanationText.text = "Use the stairs.";
            starText.text = "★";
        }
        else if (7 <= courseNumber && courseNumber <= 11) {
            explanationText.text = "The bridges will open the way.";
            starText.text = "★★";
        }
        else if (courseNumber == 12) {
            explanationText.text = "The walls will block your way.";
            starText.text = "★★";
        }
        else if (courseNumber == 13) {
            explanationText.text = "The ogre will appear.";
            starText.text = "★";
        }
        else if (courseNumber == 14) {
            explanationText.text = "The blue ogres are patrolling.";
            starText.text = "★★";
        }
        else if (courseNumber == 15) {
            explanationText.text = "There is a green ogre beyond the wall.";
            starText.text = "★★";
        }
        else if (courseNumber == 16) {
            explanationText.text = "The green ogres are strolling around happily.";
            starText.text = "★★★";
        }
        else if (courseNumber == 17) {
            explanationText.text = "Get the red ogre out of your way.";
            starText.text = "★★★";
        }
        else if (courseNumber == 18) {
            explanationText.text = "Stop the green ogre from advancing.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 19) {
            explanationText.text = "Come on. The green ogre will welcome you.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 20) {
            explanationText.text = "Let's join the red ogres in their party.";
            starText.text = "★★";
        }
        else if (courseNumber == 21) {
            explanationText.text = "The red ogres won't let you through the escape exit.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 22) {
            explanationText.text = "A lot of walls are put up.";
            starText.text = "★★";
        }
        else if (courseNumber == 23) {
            explanationText.text = "Lure the red ogre to you.";
            starText.text = "★★★";
        }
        else if (courseNumber == 24) {
            explanationText.text = "Excuse me from behind.";
            starText.text = "★★";
        }
        else if (courseNumber == 25) {
            explanationText.text = "There is only one guard, the blue ogre.";
            starText.text = "★★";
        }
        else if (courseNumber == 26) {
            explanationText.text = "The guards are four blue ogres.";
            starText.text = "★★★";
        }
        else if (courseNumber == 27) {
            explanationText.text = "You can see an escape exit behind the white ogre.";
            starText.text = "★★★";
        }
        else if (courseNumber == 28) {
            explanationText.text = "The blue ogres are cooperating.";
            starText.text = "★★★";
        }
        else if (courseNumber == 29) {
            explanationText.text = "The white ogres block your path.\nThe green ogre chases you.\n" +
                "The blue ogre guards the escape route.";
            starText.text = "★★★";
        }
        else if (courseNumber == 30) {
            explanationText.text = "You got everything?";
            starText.text = "★★";
        }
        else if (courseNumber == 31) {
            explanationText.text = "It's a one-on-one battle with the green ogre.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 32) {
            explanationText.text = "The path is limited.";
            starText.text = "★★★";
        }
        else if (courseNumber == 33) {
            explanationText.text = "Hurry up and turn.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 34) {
            explanationText.text = "The blue ogres are teaming up with each other.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 35) {
            explanationText.text = "The path is limited.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 36) {
            explanationText.text = "The green ogre is worshipped.";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 37) {
            explanationText.text = "Can you break the coordination between the green ogres?";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 38) {
            explanationText.text = "Take the bridge through the blue ogre's gap.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 39) {
            explanationText.text = "The red ogres rush you.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 40) {
            explanationText.text = "Act quickly.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 41) {
            explanationText.text = "The wall is guarded by the red ogres.";
            starText.text = "★★★";
        }
        else if (courseNumber == 42) {
            explanationText.text = "The green and red ogres welcome your visit.";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 43) {
            explanationText.text = "How do you get away?";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 44) {
            explanationText.text = "Breathe with the blue ogres.";
            starText.text = "★★★";
        }
        else if (courseNumber == 45) {
            explanationText.text = "Let's go around with the blue ogres.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 46) {
            explanationText.text = "It's up to you to play with the blue ogres.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 47) {
            explanationText.text = "Look at the breathing of the blue and green ogres.";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 48) {
            explanationText.text = "Can you escape before the red ogres arrive? No, you can't.";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 49) {
            explanationText.text = "The ogres are huddled together.";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 50) {
            explanationText.text = "The blue ogres are queuing up.";
            starText.text = "★★★★★";
        }
        else {
            courseText.text = "";
            explanationText.text = "";
            starText.text = "";
        }
    }
}
