CREATE TABLE account (
  account_id SERIAL PRIMARY KEY,
  username VARCHAR(50) NOT NULL UNIQUE,
  first_name VARCHAR(50) NOT NULL,
  last_name VARCHAR(50) NOT NULL,
  email VARCHAR(100) NOT NULL UNIQUE,
  password_hash BYTEA NOT NULL
);

CREATE TABLE friendInvite (
	invite_id SERIAL PRIMARY KEY,
	initiator_id INT REFERENCES account(account_id) ON DELETE CASCADE NOT NULL,
	invitee_id INT REFERENCES account(account_id) ON DELETE CASCADE NOT NULL,
	message VARCHAR(100),
	request_time TIMESTAMP NOT NULL,
	status INT NOT NULL
);

CREATE TABLE directMessage (
	message_id SERIAL PRIMARY KEY,
	sender_id INT REFERENCES account(account_id) ON DELETE CASCADE NOT NULL,
	receiver_id INT REFERENCES account(account_id) ON DELETE CASCADE NOT NULL,
	content VARCHAR(250) NOT NULL,
	send_time TIMESTAMP NOT NULL,
	read_time TIMESTAMP
);

CREATE TABLE raceleague (
  league_id SERIAL PRIMARY KEY,
  owner_id INT REFERENCES account(account_id) ON DELETE SET NULL,
  name VARCHAR(50) NOT NULL,
  is_public BOOLEAN NOT NULL,
  create_time TIMESTAMP NOT NULL
);

CREATE TABLE raceLeagueMembership (
	league_id INT REFERENCES raceleague(league_id) ON DELETE CASCADE,
	user_id INT REFERENCES account(account_id) ON DELETE CASCADE,
	join_time TIMESTAMP NOT NULL,
	PRIMARY KEY (league_id, user_id)
);

CREATE TABLE leagueInvite (
	invite_id SERIAL PRIMARY KEY,
	league_id INT REFERENCES raceleague(league_id) ON DELETE CASCADE NOT NULL,
	invitee_id INT REFERENCES account(account_id) ON DELETE CASCADE NOT NULL,
	request_time TIMESTAMP NOT NULL,
	status SMALLINT NOT NULL
);

CREATE TABLE track (
	track_id SERIAL PRIMARY KEY,
	name VARCHAR(50) NOT NULL,
	location VARCHAR(100) NOT NULL,
	distance NUMERIC(10, 2) NOT NULL,
	turns INT NOT NULL
);

CREATE TABLE race (
	race_id SERIAL PRIMARY KEY,
	track_id INT REFERENCES track(track_id) ON DELETE CASCADE NOT NULL,
	start_time TIMESTAMP NOT NULL
);

CREATE TABLE ballot (
	ballot_id SERIAL PRIMARY KEY,
	league_id INT REFERENCES raceleague(league_id) ON DELETE CASCADE NOT NULL,
	race_id INT REFERENCES race(race_id) ON DELETE CASCADE NOT NULL,
	user_id INT REFERENCES account(account_id) ON DELETE CASCADE NOT NULL,
	create_time TIMESTAMP NOT NULL,
	settle_time TIMESTAMP
);

CREATE TABLE team (
	team_id SERIAL PRIMARY KEY,
	name VARCHAR(50) NOT NULL,
	nationality VARCHAR(50) NOT NULL,
	create_time TIMESTAMP NOT NULL
);

CREATE TABLE driver (
	driver_id SERIAL PRIMARY KEY,
	driver_number INT NOT NULL,
	first_name VARCHAR(50) NOT NULL,
	last_name VARCHAR(50) NOT NULL,
	age INT NOT NULL,
	nationality VARCHAR(50) NOT NULL,
	height NUMERIC(5, 2) NOT NULL,
	team_id INT REFERENCES team(team_id) ON DELETE SET NULL
);

CREATE TABLE ballotContent(
	ballot_id INT REFERENCES ballot(ballot_id) ON DELETE CASCADE,
	position INT,
	driver_id INT REFERENCES driver(driver_id) ON DELETE SET NULL,
	PRIMARY KEY (ballot_id, position)
);

CREATE TABLE raceResult (
	race_id INT REFERENCES race(race_id) ON DELETE CASCADE,
	position INT,
	driver_id INT REFERENCES driver(driver_id) ON DELETE SET NULL,
	PRIMARY KEY (race_id, position)
);

/* 
   **ChatGPT generated test data**
   **slightly modified to keep consistency of foreign keys**

   Prompt:

   "the following is my postgresql initalize script. Please review the schema, 
   and create additonal lines in the initialize script to create 5 rows of test data in each table:
   
   [copy/paste of initialize script from lines 1-104]"
*/

-- Insert test data for account table
INSERT INTO account (username, first_name, last_name, email, password_hash)
VALUES 
  ('user1', 'John', 'Doe', 'john.doe@example.com', 'hash1'),
  ('user2', 'Jane', 'Smith', 'jane.smith@example.com', 'hash2'),
  ('user3', 'Bob', 'Brown', 'bob.brown@example.com', 'hash3'),
  ('user4', 'Alice', 'Green', 'alice.green@example.com', 'hash4'),
  ('user5', 'Charlie', 'Black', 'charlie.black@example.com', 'hash5');

-- Insert test data for friendInvite table
INSERT INTO friendInvite (initiator_id, invitee_id, message, request_time, status)
VALUES 
  (1, 2, 'Let’s be friends!', NOW(), 0),
  (2, 3, 'Game on!', NOW(), 1),
  (3, 4, 'Join my team?', NOW(), 0),
  (4, 5, 'Let’s race!', NOW(), 1),
  (5, 1, 'Need a teammate!', NOW(), 2);

-- Insert test data for directMessage table
INSERT INTO directMessage (sender_id, receiver_id, content, send_time)
VALUES 
  (1, 2, 'Hello Jane!', NOW()),
  (2, 3, 'Good luck!', NOW()),
  (3, 4, 'Ready to race?', NOW()),
  (4, 5, 'See you on the track!', NOW()),
  (5, 1, 'Good job!', NOW());

-- Insert test data for raceleague table
INSERT INTO raceleague (owner_id, name, is_public, create_time)
VALUES 
  (1, 'Speedsters League', true, NOW()),
  (2, 'Pro Racers League', false, NOW()),
  (3, 'Weekend Warriors', true, NOW()),
  (4, 'Champions Circuit', true, NOW()),
  (5, 'Rookie Racers', false, NOW());

-- Insert test data for raceLeagueMembership table
INSERT INTO raceLeagueMembership (league_id, user_id, join_time)
VALUES 
  (1, 1, NOW()),
  (1, 2, NOW()),
  (2, 3, NOW()),
  (3, 4, NOW()),
  (4, 5, NOW());

-- Insert test data for leagueInvite table
INSERT INTO leagueInvite (league_id, invitee_id, request_time, status)
VALUES 
  (1, 3, NOW(), 0),
  (2, 4, NOW(), 1),
  (3, 5, NOW(), 0),
  (4, 1, NOW(), 1),
  (5, 2, NOW(), 0);

-- Insert test data for track table
INSERT INTO track (name, location, distance, turns)
VALUES 
  ('Monza', 'Italy', 5.79, 11),
  ('Silverstone', 'UK', 5.89, 18),
  ('Nürburgring', 'Germany', 5.15, 16),
  ('Suzuka', 'Japan', 5.81, 18),
  ('Spa-Francorchamps', 'Belgium', 7.00, 20);

-- Insert test data for race table
INSERT INTO race (track_id, start_time)
VALUES 
  (1, NOW() + INTERVAL '1 day'),
  (2, NOW() + INTERVAL '2 days'),
  (3, NOW() + INTERVAL '3 days'),
  (4, NOW() + INTERVAL '4 days'),
  (5, NOW() + INTERVAL '5 days');

-- Insert test data for ballot table
INSERT INTO ballot (league_id, race_id, user_id, create_time)
VALUES 
  (1, 1, 1, NOW()),
  (2, 2, 2, NOW()),
  (3, 3, 3, NOW()),
  (4, 4, 4, NOW()),
  (5, 5, 5, NOW());

-- Insert test data for team table
INSERT INTO team (name, nationality, create_time)
VALUES 
  ('Ferrari', 'Italy', NOW()),
  ('Mercedes', 'Germany', NOW()),
  ('Red Bull', 'Austria', NOW()),
  ('McLaren', 'UK', NOW()),
  ('Renault', 'France', NOW());

-- Insert test data for driver table
INSERT INTO driver (driver_number, first_name, last_name, age, nationality, height, team_id)
VALUES 
  (44, 'Lewis', 'Hamilton', 38, 'UK', 1.74, 2),
  (33, 'Max', 'Verstappen', 26, 'Netherlands', 1.81, 3),
  (16, 'Charles', 'Leclerc', 26, 'Monaco', 1.80, 1),
  (4, 'Lando', 'Norris', 24, 'UK', 1.70, 4),
  (14, 'Fernando', 'Alonso', 42, 'Spain', 1.71, 5);

-- Insert test data for ballotContent table
INSERT INTO ballotContent (ballot_id, position, driver_id)
VALUES 
  (1, 1, 1),
  (2, 2, 2),
  (3, 3, 3),
  (4, 4, 4),
  (5, 5, 5);

-- Insert test data for raceResult table
INSERT INTO raceResult (race_id, position, driver_id)
VALUES 
  (1, 1, 1),
  (2, 2, 2),
  (3, 3, 3),
  (4, 4, 4),
  (5, 5, 5);
