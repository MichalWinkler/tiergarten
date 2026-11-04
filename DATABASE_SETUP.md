# Tiergarten - Database & Leaderboard Setup Guide

This guide walks you through connecting your game to a PostgreSQL database hosted on **Supabase** in less than 5 minutes.

---

## Step 1: Create your Free Supabase Database
1. Go to [supabase.com](https://supabase.com) and sign in (or create a free account).
2. Click **New project**, choose a name (e.g. `tiergarten`), enter a database password, and pick the region closest to you.
3. Click **Create new project** and wait ~1-2 minutes for it to initialize.

---

## Step 2: Create the Database Table
1. On the left sidebar of your Supabase dashboard, click the **SQL Editor** icon (`>_`).
2. Click **New query**.
3. Open `schema.sql` from your project folder, copy its contents, paste them into the SQL Editor, and click **Run**.
4. You should see `Success. No rows returned`.
   *(You can verify by clicking the **Table Editor** on the left; you will see the `players` table).*

---

## Step 3: Get your API Credentials
1. In your Supabase dashboard, click the gear icon at the bottom-left: **Project Settings** -> **API**.
2. Find the **Project URL** (e.g., `https://xyzabcdefg.supabase.co`).
3. Find the **Project API keys** -> copy the **`anon` `public`** key (starts with `eyJ...`).

---

## Step 4: Configure Unity
1. Open your Unity project.
2. Open your main game scene (`Assets/Scenes/SampleScene.unity`).
3. Create an empty GameObject in the scene (Right click in Hierarchy -> **Create Empty**), and rename it to `DatabaseManager`.
4. In the Inspector, click **Add Component** and select `SupabaseManager`.
5. Paste your:
   - **Supabase URL**
   - **Supabase Anon Key**
   - (Optional) Set your preferred **Player Name** (e.g. `Micha`).
6. Press **Play** in Unity!

---

## Testing & In-Game Overlay
- When you press **Play**, you will see a small overlay in the top-left corner:
  - Displays: **Player Name**, **Pets in Scene**, **Total Playtime**, and current **Leaderboard**.
  - Click **Save Progress** to upload your current stats to PostgreSQL.
  - Click **Refresh Board** to see remote players' scores.
  - Press the **Tab** key at any time to hide or show this overlay.
