-- ========================================================
-- TIERGARTEN - POSTGRESQL / SUPABASE DATABASE SCHEMA
-- ========================================================
-- Run this in your Supabase SQL Editor (https://supabase.com)

-- 1. Create the players table
CREATE TABLE IF NOT EXISTS players (
    player_name VARCHAR(50) PRIMARY KEY,
    pet_count INTEGER NOT NULL DEFAULT 0 CHECK (pet_count >= 0),
    playtime_seconds BIGINT NOT NULL DEFAULT 0 CHECK (playtime_seconds >= 0),
    last_online_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 2. Indexes for fast leaderboard querying
CREATE INDEX IF NOT EXISTS idx_players_pet_count ON players(pet_count DESC);
CREATE INDEX IF NOT EXISTS idx_players_playtime ON players(playtime_seconds DESC);
CREATE INDEX IF NOT EXISTS idx_players_last_online ON players(last_online_at DESC);

-- 3. Trigger to auto-update 'updated_at' column on row modification
CREATE OR REPLACE FUNCTION update_timestamp_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trigger_update_players_timestamp ON players;
CREATE TRIGGER trigger_update_players_timestamp
BEFORE UPDATE ON players
FOR EACH ROW
EXECUTE FUNCTION update_timestamp_column();

-- 4. Enable Row Level Security (RLS)
ALTER TABLE players ENABLE ROW LEVEL SECURITY;

-- 5. Policy: Allow remote game clients (using the anon key) to read and upsert player records
DROP POLICY IF EXISTS "Allow public read and write" ON players;
CREATE POLICY "Allow public read and write"
ON players
FOR ALL
TO anon
USING (true)
WITH CHECK (true);
