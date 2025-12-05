# Quick Start: Publishing BLLMT to GitHub

This is a **step-by-step guide** for publishing your BLLMT project to GitHub, even if you've never used Git before.

## Prerequisites

1. **GitHub Account**
   - Create one at https://github.com/signup
   - It's free!

2. **Git Installed**
   - Windows: Download from https://git-scm.com/download/win
   - macOS: Run `git --version` in Terminal (will prompt install if needed)

3. **Open Terminal/PowerShell**
   - Windows: Press `Win + X` ? PowerShell
   - macOS: Press `Cmd + Space` ? type "Terminal"

## Step 1: Configure Git (First Time Only)

```bash
# Set your name (will appear in commits)
git config --global user.name "Your Name"

# Set your email (use your GitHub email)
git config --global user.email "your.email@example.com"

# Verify settings
git config --list
```

## Step 2: Navigate to Your Project

```bash
# Windows
cd C:\Users\Mommans\source\repos\BLLMT

# macOS/Linux
cd ~/path/to/BLLMT
```

## Step 3: Initialize Git Repository

```bash
# Initialize git in your project folder
git init

# Check what files will be included
git status
```

You should see:
- ? Green/new files: `.cs`, `.csproj`, `.md`, scripts
- ? No `bin/`, `obj/`, or `.vs/` folders (thanks to `.gitignore`)

## Step 4: Make Your First Commit

```bash
# Add all files
git add .

# Commit with a message
git commit -m "Initial commit: BLLMT cross-platform typing assistant"

# Verify commit
git log --oneline
```

## Step 5: Create GitHub Repository

1. Go to https://github.com/new
2. Fill in:
   - **Repository name**: `bllmt` (or your choice)
   - **Description**: "Cross-platform LLM typing assistant"
   - **Visibility**: Choose Public or Private
3. **IMPORTANT**: 
   - ? **DON'T** check "Add a README"
   - ? **DON'T** check "Add .gitignore"
   - ? **DON'T** choose a license
   - (We already have these files!)
4. Click **"Create repository"**

## Step 6: Connect and Push to GitHub

After creating the repository, GitHub will show you commands. Use these:

```bash
# Add GitHub as remote (replace with YOUR repository URL)
git remote add origin https://github.com/YOUR-USERNAME/bllmt.git

# Verify remote was added
git remote -v

# Rename branch to main (if needed)
git branch -M main

# Push to GitHub
git push -u origin main
```

### If you get an error about authentication:

**GitHub now requires a Personal Access Token instead of password:**

1. Go to GitHub ? Settings ? Developer settings ? Personal access tokens ? Tokens (classic)
2. Click "Generate new token (classic)"
3. Give it a name: "BLLMT repo access"
4. Check: `repo` (all sub-options)
5. Set expiration (or "No expiration" if you trust your machine)
6. Click "Generate token"
7. **COPY THE TOKEN** (you won't see it again!)
8. When git asks for password, paste the token instead

## Step 7: Verify on GitHub

1. Go to your repository URL: `https://github.com/YOUR-USERNAME/bllmt`
2. Refresh the page
3. You should see:
   - ? All your source code
   - ? README.md displayed at bottom
   - ? LICENSE file
   - ? No `bin/` or `obj/` folders

## Step 8: Add Repository Description and Topics

1. On your repository page, click the ?? gear icon next to "About"
2. Add description:
   ```
   Cross-platform LLM typing assistant that runs in the background and types AI responses through hotkeys
   ```
3. Add topics (press Enter after each):
   - `llm`
   - `ai`
   - `typing-assistant`
   - `automation`
   - `cross-platform`
   - `dotnet`
   - `windows`
   - `macos`
4. Click "Save changes"

## Step 9: Create Your First Release (Optional)

```bash
# Tag the current version
git tag -a v1.0.0 -m "First release of BLLMT"

# Push the tag to GitHub
git push origin v1.0.0
```

This will trigger the GitHub Actions workflow to build Windows and macOS versions automatically!

To see the builds:
1. Go to your repository
2. Click "Actions" tab
3. Watch the builds run
4. When complete, check "Releases" for the built executables

## Daily Workflow (After Setup)

### Making changes:

```bash
# 1. Make your changes in Visual Studio/VS Code

# 2. See what changed
git status

# 3. Add changed files
git add .

# 4. Commit with a descriptive message
git commit -m "Add support for new LLM provider"

# 5. Push to GitHub
git push
```

### Creating a new release:

```bash
# 1. Update version number in your code
# 2. Commit the changes
git add .
git commit -m "Bump version to 1.1.0"
git push

# 3. Create and push tag
git tag -a v1.1.0 -m "Version 1.1.0 - Add XYZ feature"
git push origin v1.1.0

# 4. GitHub Actions will automatically build and create release!
```

## Common Commands Quick Reference

```bash
# Check status
git status

# Add all changes
git add .

# Commit changes
git commit -m "Description of changes"

# Push to GitHub
git push

# Pull latest changes (if working from multiple computers)
git pull

# View commit history
git log --oneline

# Create a tag
git tag -a v1.0.0 -m "Version message"

# Push tag
git push origin v1.0.0

# View all tags
git tag -l
```

## Troubleshooting

### "fatal: not a git repository"
```bash
# Make sure you're in the right folder
pwd  # or 'cd' on Windows to see current directory

# Initialize git
git init
```

### "Permission denied (publickey)"
- You need to set up SSH keys or use HTTPS with Personal Access Token
- Easier: Use HTTPS URL and Personal Access Token

### "Updates were rejected because the remote contains work"
```bash
# Pull first, then push
git pull origin main
git push origin main
```

### "Working tree has modifications"
```bash
# Commit your changes first
git add .
git commit -m "Work in progress"
git push
```

### Can't see my changes on GitHub
```bash
# Make sure you pushed
git push

# Check if you're on the right branch
git branch

# Check if commits were made
git log --oneline
```

## GitHub Features to Explore

### Issues
Track bugs and feature requests:
1. Go to "Issues" tab
2. Click "New issue"
3. Describe the bug/feature
4. Assign labels, milestones

### Pull Requests
For code review (useful with collaborators):
1. Create a branch: `git checkout -b feature-name`
2. Make changes and commit
3. Push branch: `git push origin feature-name`
4. Create Pull Request on GitHub

### Discussions
For general questions and community:
1. Enable in Settings ? Features ? Discussions
2. Users can ask questions without filing issues

### Wiki
For extended documentation:
1. Enable in Settings ? Features ? Wikis
2. Add pages for tutorials, guides, etc.

## Best Practices

? **DO:**
- Write clear commit messages
- Commit often (small, logical changes)
- Test before committing
- Use tags for versions
- Write good README documentation

? **DON'T:**
- Commit `bin/`, `obj/`, or `publish/` folders (`.gitignore` prevents this)
- Commit API keys or passwords
- Make huge commits with many unrelated changes
- Force push (`git push -f`) unless you know what you're doing

## Getting Help

- **Git Documentation**: https://git-scm.com/doc
- **GitHub Docs**: https://docs.github.com/
- **GitHub Community**: https://github.com/community
- **Stack Overflow**: Tag questions with `git` and `github`

## Success Checklist

- [ ] Git installed and configured
- [ ] Project initialized with `git init`
- [ ] First commit made
- [ ] GitHub repository created
- [ ] Code pushed to GitHub
- [ ] Repository description and topics added
- [ ] README displays correctly on GitHub
- [ ] (Optional) First release created with tag

---

**?? Congratulations! Your project is now on GitHub!**

Your BLLMT project is now:
- ? Version controlled
- ? Backed up online
- ? Ready to share
- ? Set up for collaboration
- ? Automatically building releases

**You're now a Git/GitHub user!** ??

---

*Need help? Open an issue on GitHub or check the documentation links above.*
