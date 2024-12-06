CREATE EXTENSION IF NOT EXISTS pgcrypto;

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
  create_time TIMESTAMP NOT NULL,
  invite_code VARCHAR(10) NOT NULL
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
	status INT NOT NULL
);

CREATE TABLE track (
	round_number SERIAL PRIMARY KEY,
	name VARCHAR(50) NOT NULL,
	location VARCHAR(100) NOT NULL,
	distance NUMERIC(10, 2) NOT NULL,
	turns INT NOT NULL,
	layout_image_path VARCHAR(255) NOT NULL -- Path to the image file in the repository
);

CREATE TABLE race (
	race_id SERIAL PRIMARY KEY,
	round_number INT REFERENCES track(round_number) ON DELETE CASCADE NOT NULL,
	start_time TIMESTAMP NOT NULL
);

CREATE TABLE ballot (
	ballot_id SERIAL PRIMARY KEY,
	league_id INT REFERENCES raceleague(league_id) ON DELETE CASCADE NOT NULL,
	race_id INT REFERENCES race(race_id) ON DELETE CASCADE NOT NULL,
	user_id INT REFERENCES account(account_id) ON DELETE CASCADE NOT NULL,
	create_time TIMESTAMP NOT NULL,
	settle_time TIMESTAMP,
	score INT NULL
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
	team_id INT REFERENCES team(team_id) ON DELETE SET NULL,
	headshot_path VARCHAR(255) NOT NULL, -- Path to the image file in the repository
	car_image_path VARCHAR(255) NOT NULL, -- Path to the image file in the repository
	team_image_path VARCHAR(255) NOT NULL, -- Path to the image file in the repository
	flag_image_path VARCHAR(255) NOT NULL,
	permanent_number INT
);

CREATE TABLE ballotContent(
	ballot_id INT REFERENCES ballot(ballot_id) ON DELETE CASCADE,
	position INT,
	driver_name VARCHAR(50) NULL,
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
  ('admin', 'Overtake', 'Admin', 'admin@gmail.com', digest('overtake|test', 'sha256')),
  ('user2', 'Jane', 'Smith', 'jane.smith@example.com', digest('overtake|test', 'sha256')),
  ('user3', 'Bob', 'Brown', 'bob.brown@example.com', digest('overtake|test', 'sha256')),
  ('user4', 'Alice', 'Green', 'alice.green@example.com', digest('overtake|test', 'sha256')),
  ('user5', 'Charlie', 'Black', 'charlie.black@example.com', digest('overtake|test', 'sha256'));

-- Insert test data for raceleague table
INSERT INTO raceleague (owner_id, name, is_public, create_time, invite_code)
VALUES 
  (1, 'Speedsters League', true, NOW(), 'D34RFGTUH5'),
  (2, 'Pro Racers League', false, NOW(), 'GY674EDFTH'),
  (3, 'Weekend Warriors', true, NOW(), 'DT654UJ876'),
  (4, 'Champions Circuit', true, NOW(), 'FTG54D78I9'),
  (5, 'Rookie Racers', false, NOW(), 'FT546Y3WE3');

-- Insert test data for raceLeagueMembership table
INSERT INTO raceLeagueMembership (league_id, user_id, join_time)
VALUES 
  (1, 1, NOW()),
  (1, 2, NOW()),
  (1, 3, NOW()),
  (1, 4, NOW()),
  (1, 5, NOW());

-- Insert test data for leagueInvite table
INSERT INTO leagueInvite (league_id, invitee_id, request_time, status)
VALUES 
  (1, 3, NOW(), 0),
  (2, 4, NOW(), 1),
  (3, 5, NOW(), 0),
  (4, 1, NOW(), 1),
  (5, 2, NOW(), 0);

-- Insert test data for track table

INSERT INTO track (round_number, name, location, distance, turns, layout_image_path)
VALUES 
  (16, 'Monza', 'Italy', 5.79, 11,'./assets/track_layout/monza.png'),
  (12, 'Silverstone', 'UK', 5.89, 18, './assets/track_layout/silverstone.png'),
  (21, 'Interlagos', 'Brazil', 4.31, 15, './assets/track_layout/interlagos.png'),
  (4, 'Suzuka', 'Japan', 5.81, 18, './assets/track_layout/suzuka.png'),
  (14, 'Spa-Francorchamps', 'Belgium', 7.00, 19, './assets/track_layout/spa.png'),
  (24, 'Abu Dhabi', 'UAE', 5.55, 21, './assets/track_layout/abu_dhabi.png'),
  (3, 'Melbourne', 'Australia', 5.30, 16, './assets/track_layout/melbourne.png'),
  (8, 'Monaco', 'Monaco', 3.34, 19, './assets/track_layout/monaco.png'),
  (17, 'Baku', 'Azerbaijan', 6.00, 20, './assets/track_layout/baku.png'),
  (19, 'Cota', 'USA', 5.51, 20, './assets/track_layout/cota.png'),
  (11, 'Red Bull Ring', 'Austria', 4.32, 10, './assets/track_layout/austria.png'),
  (13, 'Hungaroring', 'Hungary', 4.38, 14, './assets/track_layout/hungaroring.png'),
  (1, 'Bahrain', 'Bahrain', 5.41, 15, './assets/track_layout/bahrain.png'),
  (7, 'Imola', 'Italy', 4.93, 19, './assets/track_layout/imola.png'),
  (9, 'Montreal', 'Canada', 4.36, 14, './assets/track_layout/montreal.png'),
  (5, 'Shanghai', 'China', 5.45, 16, './assets/track_layout/shanghai.png'),
  (18, 'Singapore', 'Singapore', 5.07, 23, './assets/track_layout/singapore.png'),
  (15, 'Zandvoort', 'Netherlands', 4.26, 14, './assets/track_layout/zandvoort.png'),
  (2, 'Jeddah', 'Saudi Arabia', 6.17, 27, './assets/track_layout/jeddah.png'),
  (22, 'Las Vegas', 'USA', 6.19, 20, './assets/track_layout/las_vegas.png'),
  (20, 'Mexico City', 'Mexico', 4.30, 17, './assets/track_layout/mexico_city.png'),
  (6, 'Miami', 'USA', 5.41, 19, './assets/track_layout/miami.png'),
  (23, 'Losail', 'Quatar', 5.38, 16, './assets/track_layout/losail.png'),
  (10, 'Barcelona', 'Spain', 4.65, 16, './assets/track_layout/barcelona.png');

-- Insert test data for race table
INSERT INTO race (round_number, start_time)
VALUES
  (1,  '2024-03-02 15:00:00'), -- Bahrain
  (2,  '2024-03-09 17:00:00'), -- Jeddah
  (3,  '2024-03-24 04:00:00'), -- Melbourne
  (4,  '2024-04-07 05:00:00'), -- Suzuka
  (5,  '2024-04-21 07:00:00'), -- Shanghai
  (6,  '2024-05-05 20:00:00'), -- Miami
  (7,  '2024-05-19 13:00:00'), -- Imola
  (8,  '2024-05-26 13:00:00'), -- Monaco
  (9,  '2024-06-09 18:00:00'), -- Montreal
  (10, '2024-06-23 13:00:00'), -- Barcelona
  (11, '2024-06-30 13:00:00'), -- Red Bull Ring
  (12, '2024-07-07 14:00:00'), -- Silverstone
  (13, '2024-07-21 13:00:00'), -- Hungaroring
  (14, '2024-07-28 13:00:00'), -- Spa-Francorchamps
  (15, '2024-08-25 13:00:00'), -- Zandvoort
  (16, '2024-09-01 13:00:00'), -- Monza
  (17, '2024-09-15 11:00:00'), -- Baku
  (18, '2024-09-22 12:00:00'), -- Singapore
  (19, '2024-10-20 19:00:00'), -- Cota
  (20, '2024-10-27 20:00:00'), -- Mexico City
  (21, '2024-11-03 17:00:00'), -- Interlagos
  (22, '2024-11-23 06:00:00'), -- Las Vegas
  (23, '2024-12-01 17:00:00'), -- Losail
  (24, '2024-12-08 13:00:00'); -- Abu Dhabi


-- Insert test data for ballot table
INSERT INTO ballot (league_id, race_id, user_id, create_time, settle_time, score)
VALUES 
  (1, 1, 1, NOW(), NOW(), 15),
  (1, 1, 2, NOW(), NOW(), 85),
  (1, 2, 1, NOW(), NOW(), 125),
  (1, 2, 2, NOW(), NOW(), 0),
  (1, 3, 1, NOW(), NOW(), 25),
  (1, 3, 2, NOW(), NOW(), 55),
  (1, 4, 1, NOW(), NOW(), 75),
  (1, 4, 2, NOW(), NOW(), 905),
  (1, 5, 1, NOW(), NOW(), 55),
  (1, 5, 2, NOW(), NOW(), 150),
  (1, 22, 1, NOW(), NOW(), 0),
  (1, 22, 2, NOW(), NOW(), 0),
  (1, 22, 3, NOW(), NOW(), 0);


-- Insert test data for team table
INSERT INTO team (name, nationality, create_time)
VALUES 
  ('Ferrari', 'Italy', NOW()),
  ('Mercedes', 'Germany', NOW()),
  ('Red Bull', 'Austria', NOW()),
  ('McLaren', 'UK', NOW()),
  ('Renault', 'France', NOW());

-- Insert test data for driver table
INSERT INTO driver (driver_number, first_name, last_name, age, nationality, height, team_id, headshot_path, car_image_path, team_image_path, flag_image_path)
VALUES 
  (1, 'Max', 'Verstappen', 26, 'Netherlands', 1.81, 3, './assets/driver_headshot/verstappen.png', './assets/cars/rbr.png', './assets/teamlogos/red_bull_mini.png', './assets/country_flags/ne.svg'),  
  (11, 'Sergio', 'Perez', 31, 'Mexico', 1.73, 3, './assets/driver_headshot/perez.png', './assets/cars/rbr.png', './assets/teamlogos/red_bull_mini.png', './assets/country_flags/mx.svg'),
  (44, 'Lewis', 'Hamilton', 37, 'UK', 1.74, 2, './assets/driver_headshot/hamilton.png', './assets/cars/mercedes.png', './assets/teamlogos/mercedes_mini.png', './assets/country_flags/uk.svg'),
  (63, 'George', 'Russell', 24, 'UK', 1.85, 2, './assets/driver_headshot/russell.png', './assets/cars/mercedes.png', './assets/teamlogos/mercedes_mini.png', './assets/country_flags/uk.svg'),
  (77, 'Valtteri', 'Bottas', 32, 'Finland', 1.73, 2, './assets/driver_headshot/bottas.png', './assets/cars/kick_sauber.png', './assets/teamlogos/kick_sauber.png', './assets/country_flags/fi.svg'),
  (24, 'Guanyu', 'Zhou', 22, 'China', 1.80, 5, './assets/driver_headshot/zhou.png', './assets/cars/kick_sauber.png', './assets/teamlogos/kick_sauber.png', './assets/country_flags/ch.svg'),
  (4, 'Lando', 'Norris', 24, 'UK', 1.77, 4, './assets/driver_headshot/norris.png', './assets/cars/mcLaren.png', './assets/teamlogos/mclaren_mini.png', './assets/country_flags/uk.svg'),
  (81, 'Oscar', 'Piastri', 20, 'Australia', 1.80, 4, './assets/driver_headshot/piastri.png', './assets/cars/mcLaren.png', './assets/teamlogos/mclaren_mini.png', './assets/country_flags/au.svg'),
  (14, 'Fernando', 'Alonso', 40, 'Spain', 1.71, 4, './assets/driver_headshot/alonso.png', './assets/cars/aston_martin.png', './assets/teamlogos/aston_martin_mini.png', './assets/country_flags/sp.svg'),
  (18, 'Lance', 'Stroll', 23, 'Canada', 1.85, 4, './assets/driver_headshot/stroll.png', './assets/cars/aston_martin.png', './assets/teamlogos/aston_martin_mini.png', './assets/country_flags/ca.svg'),
  (55, 'Carlos', 'Sainz', 27, 'Spain', 1.75, 1, './assets/driver_headshot/sainz.png', './assets/cars/ferrari.png', './assets/teamlogos/ferrari_mini.png', './assets/country_flags/sp.svg'),
  (16, 'Charles', 'Leclerc', 24, 'Monaco', 1.80, 1, './assets/driver_headshot/leclerc.png', './assets/cars/ferrari.png', './assets/teamlogos/ferrari_mini.png', './assets/country_flags/mn.svg'),
  (10, 'Pierre', 'Gasly', 25, 'France', 1.74, 5, './assets/driver_headshot/gasly.png', './assets/cars/alpine.png', './assets/teamlogos/alpine_mini.png', './assets/country_flags/fr.svg'),
  (31, 'Esteban', 'Ocon', 25, 'France', 1.86, 5, './assets/driver_headshot/ocon.png', './assets/cars/alpine.png', './assets/teamlogos/alpine_mini.png', './assets/country_flags/fr.svg'),
  (27, 'Nico', 'Hulkenberg', 34, 'Germany', 1.84, 5, './assets/driver_headshot/hulkenberg.png', './assets/cars/haas.png', './assets/teamlogos/haas_mini.png', './assets/country_flags/ger.svg'),
  (20, 'Kevin', 'Magnussen', 29, 'Denmark', 1.74, 5, './assets/driver_headshot/magnussen.png', './assets/cars/haas.png', './assets/teamlogos/haas_mini.png', './assets/country_flags/de.svg'),
  (22, 'Yuki', 'Tsunoda', 21, 'Japan', 1.59, 5, './assets/driver_headshot/tsunoda.png', './assets/cars/rb.png', './assets/teamlogos/rb_mini.png', './assets/country_flags/jp.svg'),
  (30, 'Liam', 'Lawson', 19, 'New Zealand', 1.75, 5, './assets/driver_headshot/lawson.png', './assets/cars/rb.png', './assets/teamlogos/rb_mini.png', './assets/country_flags/nz.svg'),
  (23, 'Alex', 'Albon', 25, 'Thailand', 1.86, 5, './assets/driver_headshot/albon.png', './assets/cars/williams.png', './assets/teamlogos/williams_mini.png', './assets/country_flags/th.svg'),
  (43, 'Franco', 'Colapinto', 18, 'Argentina', 1.75, 5, './assets/driver_headshot/colapinto.png', './assets/cars/williams.png', './assets/teamlogos/williams_mini.png', './assets/country_flags/ar.svg'),
  (38, 'Oliver', 'Bearman', 19, 'UK', 1.85, 5, './assets/driver_headshot/bearman.png', './assets/cars/haas.png', './assets/teamlogos/haas_mini.png', './assets/country_flags/uk.svg'),
  (61, 'Jack', 'Doohan', 21, 'Australia', 1.83, 5, './assets/driver_headshot/doohan.png', './assets/cars/alpine.png', './assets/teamlogos/alpine_mini.png', './assets/country_flags/au.svg');


-- Insert test data for driver table (permanent numbers)
UPDATE driver
	set permanent_number = 33
	WHERE driver_id = 1;

UPDATE driver
	set permanent_number = 40
	WHERE driver_id = 18;

-- Insert test data for ballotContent table
INSERT INTO ballotContent (ballot_id, position, driver_name) 
VALUES
(1, 1, 'Valtteri Bottas'),
(1, 2, 'Esteban Ocon'),
(1, 3, 'Fernando Alonso'),
(1, 4, 'Lance Stroll'),
(1, 5, 'Alexander Albon'),
(1, 6, 'Nico Hülkenberg'),
(1, 7, 'Max Verstappen'),
(1, 8, 'Oliver Bearman'),
(1, 9, 'Charles Leclerc'),
(1, 10, 'Yuki Tsunoda'),
(2, 1, 'Lando Norris'),
(2, 2, 'Max Verstappen'),
(2, 3, 'Yuki Tsunoda'),
(2, 4, 'Sergio Pérez'),
(2, 5, 'Valtteri Bottas'),
(2, 6, 'Alexander Albon'),
(2, 7, 'Oscar Piastri'),
(2, 8, 'Esteban Ocon'),
(2, 9, 'Nico Hülkenberg'),
(2, 10, 'Oliver Bearman'),
(3, 1, 'Charles Leclerc'),
(3, 2, 'Oscar Piastri'),
(3, 3, 'Lando Norris'),
(3, 4, 'Valtteri Bottas'),
(3, 5, 'Yuki Tsunoda'),
(3, 6, 'Lance Stroll'),
(3, 7, 'Lewis Hamilton'),
(3, 8, 'Oliver Bearman'),
(3, 9, 'Max Verstappen'),
(3, 10, 'Alexander Albon'),
(4, 1, 'Alexander Albon'),
(4, 2, 'Lance Stroll'),
(4, 3, 'Charles Leclerc'),
(4, 4, 'Oliver Bearman'),
(4, 5, 'Esteban Ocon'),
(4, 6, 'Sergio Pérez'),
(4, 7, 'Yuki Tsunoda'),
(4, 8, 'Pierre Gasly'),
(4, 9, 'Valtteri Bottas'),
(4, 10, 'Lando Norris'),
(5, 1, 'George Russell'),
(5, 2, 'Lewis Hamilton'),
(5, 3, 'Esteban Ocon'),
(5, 4, 'Nico Hülkenberg'),
(5, 5, 'Yuki Tsunoda'),
(5, 6, 'Lance Stroll'),
(5, 7, 'Sergio Pérez'),
(5, 8, 'Guanyu Zhou'),
(5, 9, 'Pierre Gasly'),
(5, 10, 'Valtteri Bottas'),
(6, 1, 'Fernando Alonso'),
(6, 2, 'Lance Stroll'),
(6, 3, 'Max Verstappen'),
(6, 4, 'Charles Leclerc'),
(6, 5, 'Oscar Piastri'),
(6, 6, 'Alexander Albon'),
(6, 7, 'Oliver Bearman'),
(6, 8, 'Yuki Tsunoda'),
(6, 9, 'Guanyu Zhou'),
(6, 10, 'Lewis Hamilton'),
(7, 1, 'Oscar Piastri'),
(7, 2, 'Oliver Bearman'),
(7, 3, 'Lewis Hamilton'),
(7, 4, 'Pierre Gasly'),
(7, 5, 'Guanyu Zhou'),
(7, 6, 'Nico Hülkenberg'),
(7, 7, 'Fernando Alonso'),
(7, 8, 'Charles Leclerc'),
(7, 9, 'Max Verstappen'),
(7, 10, 'Alexander Albon'),
(8, 1, 'Lewis Hamilton'),
(8, 2, 'Sergio Pérez'),
(8, 3, 'Fernando Alonso'),
(8, 4, 'George Russell'),
(8, 5, 'Yuki Tsunoda'),
(8, 6, 'Oscar Piastri'),
(8, 7, 'Max Verstappen'),
(8, 8, 'Alexander Albon'),
(8, 9, 'Oliver Bearman'),
(8, 10, 'Nico Hülkenberg'),
(9, 1, 'Valtteri Bottas'),
(9, 2, 'Yuki Tsunoda'),
(9, 3, 'Lando Norris'),
(9, 4, 'Guanyu Zhou'),
(9, 5, 'Oscar Piastri'),
(9, 6, 'Esteban Ocon'),
(9, 7, 'Nico Hülkenberg'),
(9, 8, 'Lewis Hamilton'),
(9, 9, 'Oliver Bearman'),
(9, 10, 'Alexander Albon'),
(10, 1, 'Max Verstappen'),
(10, 2, 'Oliver Bearman'),
(10, 3, 'Fernando Alonso'),
(10, 4, 'Lance Stroll'),
(10, 5, 'Nico Hülkenberg'),
(10, 6, 'Guanyu Zhou'),
(10, 7, 'Esteban Ocon'),
(10, 8, 'Valtteri Bottas'),
(10, 9, 'Yuki Tsunoda'),
(10, 10, 'George Russell');

-- Insert test data for raceResult table
INSERT INTO raceResult (race_id, position, driver_id)
VALUES 
  (1, 1, 1),
  (2, 2, 2),
  (3, 3, 3),
  (4, 4, 4),
  (5, 5, 5);
