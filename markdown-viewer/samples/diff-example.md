# Code Diff Highlighting Example

## Basic Diff

```diff
- This line was removed
+ This line was added
  This line is unchanged
```

## Real-World Example

```diff
public class Calculator
{
-   public int Add(int a, int b)
+   public double Add(double a, double b)
    {
-       return a + b;
+       return Math.Round(a + b, 2);
    }
}
```

## Configuration Changes

```diff
{
  "name": "MarkdownViewer",
-  "version": "1.7.0",
+  "version": "1.8.0",
  "features": [
-    "syntax-highlighting"
+    "syntax-highlighting",
+    "emoji-support",
+    "diff-highlighting"
  ]
}
```

## Git Diff Example

```diff
diff --git a/src/app.js b/src/app.js
index 1234567..abcdefg 100644
--- a/src/app.js
+++ b/src/app.js
@@ -1,7 +1,8 @@
 function greet(name) {
-    console.log("Hello " + name);
+    console.log(`Hello ${name}!`);
 }

-greet("World");
+const userName = "World";
+greet(userName);
```

## Multi-File Diff

```diff
--- a/README.md
+++ b/README.md
@@ -1,5 +1,6 @@
 # MyProject

-Simple project
+Enhanced project with new features

-## Installation
+## Installation & Setup
+Run `npm install` to get started
```

## CSS Changes

```diff
.button {
-    background: #ccc;
-    color: black;
+    background: #4A90E2;
+    color: white;
+    padding: 10px 20px;
+    border-radius: 4px;
}
```

## Python Example

```diff
def calculate_total(items):
-    total = 0
-    for item in items:
-        total += item.price
-    return total
+    return sum(item.price for item in items)
```

## SQL Changes

```diff
-SELECT * FROM users WHERE active = 1;
+SELECT id, name, email
+FROM users
+WHERE active = 1
+ORDER BY created_at DESC;
```
