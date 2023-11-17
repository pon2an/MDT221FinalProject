﻿using System;
using System.Collections.Generic;

public enum Visibility {
    PRIVATE,
    PUBLIC,
}
public enum Gender {
    MALE,
    FEMALE,
    OTHER,
}

public class Grade {
    public int GradeValue { get; set; }
    public string CourseName { get; set; }
}

public class Block {
    public string BlockName { get; set; }
    public List<Grade> Grades { get; set; }

    public Block() {
        Grades = new List<Grade>();
    }

    public void AddGrade(int gradeValue, string courseName) {
        Grade newGrade = new Grade {
            GradeValue = gradeValue,
            CourseName = courseName
        };
        Grades.Add(newGrade);
    }
}

public abstract class Post {
    public string Content { get; set; }
    public User Author { get; set; }
    public Visibility PostVisibility { get; set; }

    public abstract void Display();
}

public class TextPost : Post {
    public override void Display() {
        Console.WriteLine("Text Post by " + Author.UserName + ": " + Content);
    }
}

public class DatingPost : Post {
    public Gender SeekingGender { get; set; }

    public override void Display() {
        Console.WriteLine($"Dating Post by {Author.UserName}: {Content} (Seeking: {SeekingGender})");
    }
}

public class User {
    public string UserName { get; set; }

    private List<Post> _postList = new List<Post>();
    private List<User> _followList = new List<User>();
    private List<Block> _blocks = new List<Block>();

    public List<User> GetFollowList() {
        List<User> resultList = new List<User>();
        foreach (User user in _followList) {
            resultList.Add(user);
        }
        return resultList;
    }

    public void FollowUser(User userToFollow) {
        if (_followList.Contains(userToFollow)) {
            Console.WriteLine(this.UserName + " has already followed " + userToFollow.UserName);
            return;
        }

        _followList.Add(userToFollow);
        Console.WriteLine(this.UserName + " follows " + userToFollow.UserName);
    }

    public List<Post> FetchPublicPost() {
        List<Post> resultList = new List<Post>();
        foreach (Post post in _postList) {
            if (post.PostVisibility == Visibility.PUBLIC) {
                resultList.Add(post);
            }
        }
        return resultList;
    }

    public void AddTextPost(string content, Visibility visibility) {
        TextPost newPost = new TextPost {
            Author = this,
            PostVisibility = visibility,
            Content = content
        };
        _postList.Add(newPost);
    }

    public void AddDatingPost(string content, Visibility visibility, Gender seekingGender) {
        DatingPost newPost = new DatingPost {
            Author = this,
            PostVisibility = visibility,
            Content = content,
            SeekingGender = seekingGender
        };
        _postList.Add(newPost);
    }

    public List<Block> Blocks {
        get { return _blocks; }
    }

    public void AddBlock(string blockName) {
        Block newBlock = new Block {
            BlockName = blockName
        };
        _blocks.Add(newBlock);
    }

    public void AddGradeToBlock(string blockName, int gradeValue, string courseName) {
        Block block = _blocks.Find(b => b.BlockName == blockName);
        if (block != null) {
            block.AddGrade(gradeValue, courseName);
        }
        else {
            Console.WriteLine("Block not found.");
        }
    }
}

public class SocialMediaApp {
    private Dictionary<string, User> _userDictionary = new Dictionary<string, User>();

    public void ShowUserFeed(User viewer) {
        int MAX_POST_PER_USER = 2;
        int MAX_POST_AMOUNT = 10;

        List<Post> postToDisplayList = new List<Post>();
        List<User> followList = viewer.GetFollowList();
        foreach (User user in followList) {
            if (postToDisplayList.Count >= MAX_POST_AMOUNT) {
                break;
            }

            int postCount = 0;
            foreach (Post post in user.FetchPublicPost()) {
                if (postCount >= MAX_POST_PER_USER || postToDisplayList.Count >= MAX_POST_AMOUNT) {
                    break;
                }
                postToDisplayList.Add(post);
                ++postCount;
            }
        }

        Console.WriteLine("-- Show " + viewer.UserName + " feed ----");
        foreach (Post post in postToDisplayList) {
            post.Display();
        }
        Console.WriteLine("----");
    }
    
    public void MatchUsers(User user) {
        List<User> potentialMatches = new List<User>();

        foreach (KeyValuePair<string, User> kvp in _userDictionary) {
            if (kvp.Value != user && kvp.Value.GetFollowList().Contains(user) && user.GetFollowList().Contains(kvp.Value)) {
                potentialMatches.Add(kvp.Value);
            }
        }

        Console.WriteLine($"Potential matches for {user.UserName}:");
        foreach (User match in potentialMatches) {
            Console.WriteLine($"- {match.UserName}");
        }
    }

    public void ShowUserWall(User userToDisplay) {
        List<Post> postToDisplayList = userToDisplay.FetchPublicPost();

        Console.WriteLine("---- Show " + userToDisplay.UserName + " wall ---");
        foreach (Post post in postToDisplayList) {
            post.Display();
        }
        Console.WriteLine("---");
    }

    public User GetUser(string userName) {
        if (!_userDictionary.ContainsKey(userName)) {
            Console.WriteLine("Username " + userName + " does not exist");
            return null;
        }

        return _userDictionary[userName];
    }

    public void RegisterUser(string userName) {
        if (_userDictionary.ContainsKey(userName)) {
            Console.WriteLine("Username " + userName + " already registered");
            return;
        }

        User newUser = new User {
            UserName = userName
        };
        _userDictionary.Add(userName, newUser);
        Console.WriteLine("Register user " + newUser.UserName);
    }
}

public class Program {
    public static void Main(string[] args) {
        SocialMediaApp socialMediaApp = new SocialMediaApp();

        socialMediaApp.RegisterUser("Bob");
        socialMediaApp.RegisterUser("Alice");
        socialMediaApp.RegisterUser("Fragile");

        socialMediaApp.GetUser("Bob").AddTextPost("This is fine.", Visibility.PUBLIC);
        socialMediaApp.GetUser("Fragile").AddTextPost("A cryptobiote a day keeps the Timefall away.", Visibility.PUBLIC);

        socialMediaApp.GetUser("Bob").AddDatingPost("Buy dog food", Visibility.PRIVATE, Gender.FEMALE);

        socialMediaApp.ShowUserWall(socialMediaApp.GetUser("Bob"));
        socialMediaApp.ShowUserWall(socialMediaApp.GetUser("Alice"));

        socialMediaApp.GetUser("Bob").FollowUser(socialMediaApp.GetUser("Alice"));
        socialMediaApp.GetUser("Bob").FollowUser(socialMediaApp.GetUser("Fragile"));

        socialMediaApp.ShowUserFeed(socialMediaApp.GetUser("Bob"));

        socialMediaApp.MatchUsers(socialMediaApp.GetUser("Bob"));

        // เพิ่ม Block และ Grade ให้กับ User
        socialMediaApp.GetUser("Bob").AddBlock("Block A");
        socialMediaApp.GetUser("Bob").AddGradeToBlock("Block A", 85, "Math");

        // แสดง Grades & Blocks ของ User
        Console.WriteLine("-- Show Grades & Blocks for Bob ----");
        foreach (Block block in socialMediaApp.GetUser("Bob").Blocks) {
            Console.WriteLine($"Block: {block.BlockName}");
            foreach (Grade grade in block.Grades) {
                Console.WriteLine($"- Course: {grade.CourseName}, Grade: {grade.GradeValue}");
            }
        }
        Console.WriteLine("----");
    }
}
