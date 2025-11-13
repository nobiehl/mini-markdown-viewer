---
description: Add a feature idea to ROADMAP.md using a background agent
---

Launch a background agent to add this feature idea to docs/ROADMAP.md:

**Feature Idea:** {idea}

**Agent Instructions:**

1. Read the current docs/ROADMAP.md file
2. Analyze the structure and find the best section to add this idea:
   - If it's a new feature → Add to appropriate version section or "Backlog"
   - If it's an enhancement → Add to "Enhancement Ideas"
   - If it's technical debt → Add to "Technical Debt"
3. Add the feature idea with proper formatting:
   - Use ⏳ status (Not started)
   - Include a brief description
   - Estimate complexity if obvious (Low/Medium/High)
   - Follow existing ROADMAP.md formatting
4. Report back with:
   - Where you added it (section name)
   - The formatted entry you created
   - No need to show the entire file

**Important:**
- Keep it brief and concise
- Don't commit changes (just edit the file)
- Match the existing ROADMAP.md style
- If unsure about placement, add to "Backlog" section

Use the Task tool with subagent_type="general-purpose" to execute this in the background.
