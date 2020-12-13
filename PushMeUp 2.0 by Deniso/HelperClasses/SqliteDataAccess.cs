using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushMeUp_2._0_by_Deniso
{
    public class SqliteDataAccess
    {
        public static List<Exercise> LoadExercises()
        {
            string query = @"SELECT e.Id, e.UserId, e.Name, e.Description, e.Image, mg.Name, t.Name FROM Exercise e 
                            LEFT JOIN Exercise_MuscleGroups emg ON e.Id = emg.ExerciseId
                            LEFT JOIN MuscleGroup mg on emg.MuscleGroupId = mg.Id
                            LEFT JOIN Exercise_ExerciseTypes et ON e.Id = et.ExerciseId
                            LEFT JOIN ExerciseType t ON et.ExerciseTypeId = t.Id
                            WHERE UserId == " + Session.LoggedUser.Id;

            Dictionary<int, Exercise> lookup = new Dictionary<int, Exercise>();
            using (IDbConnection connection = new SQLiteConnection(Helper.LoadConnectionString()))
            {

                var result = connection.Query<Exercise, string, string, Exercise>(query, (e, muscle, type) =>
                {
                    // Check if we have already stored the Exercise in the dictionary
                    if (!lookup.TryGetValue(e.Id, out Exercise exercise))
                    {
                        // The dictionary doesnt have that Exercise 
                        // Add it to the dictionary
                        lookup.Add(e.Id, e);
                        exercise = e;
                    }

                    exercise.TargetedMuscles.Add(muscle);
                    exercise.ExerciseTypes.Add(type);
                    return exercise;

                }, splitOn: "Name");


                List<Exercise> exercises = result.Distinct().ToList();
                exercises.ForEach(x => x.TargetedMuscles = x.TargetedMuscles.Distinct().ToList());
                exercises.ForEach(x => x.ExerciseTypes = x.ExerciseTypes.Distinct().ToList());
                return exercises;
            }
        }

        internal static List<UserStat> LoadUserStats(User _loggedUser)
        {
            List<UserStat> userStats;
            string queryUserStat = $"SELECT stat.Id, stat.Day, stat.StartingTime, stat.FinishingTime, stat.SelfRating FROM UserStat stat WHERE UserId == {_loggedUser.Id}";
            string queryWorkout = $"SELECT stat.CompletedWorkoutId FROM UserStat stat WHERE Id ==";
            using (IDbConnection connection = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                userStats = connection.Query<UserStat>(queryUserStat).ToList();

                for (int i = 0; i < userStats.Count; i++)
                {
                    int workoutId = connection.Query<int>(queryWorkout + userStats[i].Id).ToList().Single();
                    userStats[i].completedWorkout = LoadWorkout(workoutId);
                }
            }

            return userStats;
        }

        internal static void DeleteUser(int _id)
        {
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute($"PRAGMA foreign_keys = ON; DELETE FROM User WHERE id = {_id}");
            }
        }

        internal static void DeleteWorkout(int _id)
        {
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute($"PRAGMA foreign_keys = ON; DELETE FROM Workout WHERE id = {_id}");
            }
        }

        public static Exercise LoadExercise(int? id)
        {
            if (id == null) return null;

            string query = String.Format(@"SELECT e.Id, e.Name, e.Description, e.Image, mg.Name, t.Name FROM Exercise e 
                            LEFT JOIN Exercise_MuscleGroups emg ON e.Id = emg.ExerciseId
                            LEFT JOIN MuscleGroup mg on emg.MuscleGroupId = mg.Id
                            LEFT JOIN Exercise_ExerciseTypes et ON e.Id = et.ExerciseId
                            LEFT JOIN ExerciseType t ON et.ExerciseTypeId = t.Id
							WHERE e.Id == {0};", id);

            IEnumerable<Exercise> result;
            Dictionary<int, Exercise> lookup = new Dictionary<int, Exercise>();
            using (IDbConnection connection = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                result = connection.Query<Exercise, string, string, Exercise>(query, (e, muscle, type) =>
                {
                    // Check if we have already stored the Exercise in the dictionary
                    if (!lookup.TryGetValue(e.Id, out Exercise exercise))
                    {
                        // The dictionary doesnt have that Exercise 
                        // Add it to the dictionary
                        lookup.Add(e.Id, e);
                        exercise = e;
                    }

                    if(muscle != null)
                    {
                        exercise.TargetedMuscles.Add(muscle);
                    }
                    if(type != null)
                    {
                        exercise.ExerciseTypes.Add(type);
                    }

                    return exercise;

                }, splitOn: "Name");
            }
            List<Exercise> exercises = result.Distinct().ToList();
            exercises.ForEach(x => x.TargetedMuscles = x.TargetedMuscles.Distinct().ToList());
            exercises.ForEach(x => x.ExerciseTypes = x.ExerciseTypes.Distinct().ToList());

            if(exercises.Count > 0)
            {
                return exercises[0];
            }
            else
            {
                return null;
            }
        }

        public static bool DoesExerciseExist(string name)
        {
            string query = String.Format(@"SELECT Id FROM Exercise WHERE Name == '{0}';", name);

            using (IDbConnection connection = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                var result = connection.Query<int>(query).ToList();

                return (result.Count > 0);
            }
        }

        internal static void SaveWorkoutItem(WorkoutItem _workoutItem, Workout _workout)
        {
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute($"insert into WorkoutItem(Duration, Distance, Reps, EquipmentUsed, ItemTypeId, ExerciseId, WorkoutId) values ('{_workoutItem.Duration}', '{_workoutItem.Distance}', '{_workoutItem.Reps}', " +
                    $"'{_workoutItem.EquipmentUsed}', '{GetIdFromItemType(_workoutItem.ItemType)}', {(_workoutItem.Exercise.Id == 0 ? "NULL" : _workoutItem.Exercise.Id.ToString())}, '{_workout.Id}')");
            }
        }

        internal static void UpdateExercise(int exerciseId, Exercise exercise, List<string> muscleGroups, List<string> exerciseTypes)
        {
            string queryRemove = $"PRAGMA foreign_keys = ON; DELETE FROM Exercise WHERE Id = {exercise.Id}";
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute(queryRemove);

                // now re-add the exercise
                SaveExercise(exerciseId, exercise, muscleGroups, exerciseTypes);
            }
        }

        internal static void SaveBMIHistory(int _userId, double _height, double _weight)
        {
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute($"insert into UserBMIHistory(UserId, Height, Weight, Date, Time) values ('{_userId}', '{_height}', '{_weight}', '{DateTime.Today.ToShortDateString()}', '{DateTime.Now.ToShortDateString()}')");
            }
        }


        internal static List<UserBMI> GetBMIHistoryOfUser(int _userId)
        {
            List<UserBMI> userBMIHistory;
            string query = $"SELECT Height, Weight, Date, Time FROM UserBMIHistory WHERE UserId == {_userId}";
            using (IDbConnection connection = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                userBMIHistory = connection.Query<UserBMI>(query).ToList();
            }

            return userBMIHistory;
        }

        /// <summary>
        /// Saves exercise with a new ID and returns exercise added (with lists loaded in)
        /// </summary>
        public static Exercise SaveExercise(Exercise _exercise, List<string> _muscleGroups, List<string> _exerciseTypes)
        {
            int id;
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute($"insert into Exercise(UserId, Name, Description, Image) values ({Session.LoggedUser.Id}, '{_exercise.Name.Replace("'", "''")}', '{_exercise.Description.Replace("'", "''")}', '{_exercise.Image.Replace("'", "''")}')");

                foreach (string muscleGroup in _muscleGroups)
                {
                    cnn.Execute($"INSERT INTO Exercise_MuscleGroups(ExerciseId, MuscleGroupId) values ((SELECT MAX(id) FROM Exercise), {GetIdFromMuscleGroup(muscleGroup)})");
                }
                foreach (string exerciseType in _exerciseTypes)
                {
                    cnn.Execute($"INSERT INTO Exercise_ExerciseTypes(ExerciseId, ExerciseTypeId) values ((SELECT MAX(id) FROM Exercise), {GetIdFromExerciseType(exerciseType)})");
                }

                id = cnn.Query<int>("SELECT max(Id) FROM Exercise").Single();
                
            }

            return LoadExercise(id);
        }

        internal static void UpdateWorkoutItem(WorkoutItem _workoutItem)
        {
            string query = $"Update WorkoutItem SET Duration='{_workoutItem.Duration}', Distance='{_workoutItem.Distance}', Reps='{_workoutItem.Reps}', EquipmentUsed='{_workoutItem.EquipmentUsed}', ItemTypeId='{GetIdFromItemType(_workoutItem.ItemType)}' " +
                $"WHERE Id = {_workoutItem.Id}";

            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute(query);
            }
        }

        /// <summary>
        /// Saves exercise with the same ID
        /// </summary>
        public static void SaveExercise(int exerciseId, Exercise _exercise, List<string> _muscleGroups, List<string> _exerciseTypes)
        {
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute($"insert into Exercise(Id, UserId, Name, Description, Image) values ({exerciseId}, {Session.LoggedUser.Id}, '{_exercise.Name.Replace("'", "''")}', '{_exercise.Description.Replace("'", "''")}', '{_exercise.Image.Replace("'", "''")}')");

                foreach (string muscleGroup in _muscleGroups)
                {
                    cnn.Execute($"INSERT INTO Exercise_MuscleGroups(ExerciseId, MuscleGroupId) values ({exerciseId}, {GetIdFromMuscleGroup(muscleGroup)})");
                }
                foreach (string exerciseType in _exerciseTypes)
                {
                    cnn.Execute($"INSERT INTO Exercise_ExerciseTypes(ExerciseId, ExerciseTypeId) values ({exerciseId}, {GetIdFromExerciseType(exerciseType)})");
                }
            }
        }

        public static void DeleteExercise(int _id)
        {
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute($"PRAGMA foreign_keys = ON; DELETE FROM Exercise WHERE id = {_id}");
            }
        }

        public static List<string> LoadMuscleGroups()
        {
            List<string> muscleGroups;
            string query = "SELECT name FROM MuscleGroup";
            using (IDbConnection connection = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                muscleGroups = connection.Query<string>(query).ToList();
            }

            return muscleGroups;
        }

        public static List<string> LoadExerciseTypes()
        {
            List<string> exerciseTypes;
            string query = "SELECT name FROM ExerciseType";
            using (IDbConnection connection = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                exerciseTypes = connection.Query<string>(query).ToList();
            }

            return exerciseTypes;
        }

        public static List<Workout> LoadWorkouts()
        {
            List<Workout> workouts = new List<Workout>();
            string queryWorkout = @"SELECT * FROM Workout WHERE UserId == " + Session.LoggedUser.Id;
            string queryWorkoutItem = @"SELECT wi.Id, wi.Duration, wi.Distance, wi.Reps, wi.EquipmentUsed, wt.ItemType FROM WorkoutItem wi
                                    LEFT JOIN WorkoutItemType wt on wi.ItemTypeId = wt.Id
                                    WHERE wi.WorkoutId ==";
            string queryExerciseId = @"SELECT wi.ExerciseId FROM WorkoutItem wi
                                    WHERE wi.WorkoutId ==";

            using (IDbConnection connection = new SQLiteConnection(Helper.LoadConnectionString()))
            {

                var result = connection.Query<Workout>(queryWorkout);
                workouts = result.ToList();

                for (int i = 0; i < workouts.Count; i++)
                {
                    List<WorkoutItem> workoutItems = connection.Query<WorkoutItem>(queryWorkoutItem + workouts[i].Id).ToList();
                    List<int?> exerciseIds = connection.Query<int?>(queryExerciseId + workouts[i].Id).ToList();

                    for (int j = 0; j < workoutItems.Count; j++)
                    {
                        workoutItems[j].Exercise = LoadExercise(exerciseIds[j]);
                    }

                    workouts[i].WorkoutItems = workoutItems;
                }

                return workouts;
            }
        }

        public static Workout LoadWorkout(int _id)
        {
            Workout workout = new Workout();
            string queryWorkout = $"SELECT * FROM Workout WHERE Id == {_id};";
            string queryWorkoutItem = @"SELECT wi.Id, wi.Duration, wi.Distance, wi.Reps, wi.EquipmentUsed, wt.ItemType FROM WorkoutItem wi
                                    LEFT JOIN WorkoutItemType wt on wi.ItemTypeId = wt.Id
                                    WHERE wi.WorkoutId ==";
            string queryExerciseId = @"SELECT wi.ExerciseId FROM WorkoutItem wi
                                    WHERE wi.WorkoutId ==";

            using (IDbConnection connection = new SQLiteConnection(Helper.LoadConnectionString()))
            {

                var result = connection.Query<Workout>(queryWorkout);
                workout = result.ToList()[0];

                
                List<WorkoutItem> workoutItems = connection.Query<WorkoutItem>(queryWorkoutItem + workout.Id).ToList();
                List<int?> exerciseIds = connection.Query<int?>(queryExerciseId + workout.Id).ToList();

                for (int j = 0; j < workoutItems.Count; j++)
                {
                    workoutItems[j].Exercise = LoadExercise(exerciseIds[j]);
                }

                workout.WorkoutItems = workoutItems;
                

                return workout;
            }
        }

        public static List<string> LoadWorkoutItemTypes()
        {
            List<string> muscleGroups;
            string query = "SELECT ItemType FROM WorkoutItemType";
            using (IDbConnection connection = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                muscleGroups = connection.Query<string>(query).ToList();
            }

            return muscleGroups;
        }

        /// <summary>
        /// Removes all WorkoutItems related to a specific workout, then inserts the list of workoutitems.
        /// </summary>
        public static void UpdateWorkout(Workout _workout)
        {

            string queryRemove = $"DELETE FROM WorkoutItem WHERE WorkoutId = {_workout.Id}";
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute(queryRemove);
                foreach (WorkoutItem _workoutItem in _workout.WorkoutItems)
                {
                    int exerciseId = 0;
                    if (_workoutItem.Exercise != null) exerciseId = _workoutItem.Exercise.Id;
                    string queryUpdate = $"insert into WorkoutItem(Duration, Distance, Reps, EquipmentUsed, ItemTypeId, ExerciseId, WorkoutId) values('{_workoutItem.Duration}', '{_workoutItem.Distance}', '{_workoutItem.Reps}', " +
                    $"'{_workoutItem.EquipmentUsed}', '{GetIdFromItemType(_workoutItem.ItemType)}', {(exerciseId == 0 ? "NULL" : exerciseId.ToString())}, '{_workout.Id}')";
                    cnn.Execute(queryUpdate);

                }
            }
        }

        public static void UpdateWorkoutName(int workoutId, string newName)
        {
            string query = $"Update Workout SET Name = '{newName.Replace("'", "''")}' WHERE Id = {workoutId}";

            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute(query);
            }
        }

        internal static void SaveWorkout(string _name)
        {
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute($"insert into Workout(UserId, Name) values ('{Session.LoggedUser.Id}', '{_name}')");
            }
        }


        public static void SaveCompletedWorkout(int _workoutId, int _selfRating, DateTime startTime, DateTime endTime)
        {
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute($"insert into UserStat(UserId, Day, StartingTime, FinishingTime, SelfRating, CompletedWorkoutId) values ('{Session.LoggedUser.Id}', '{endTime.ToShortDateString()}', '{startTime.ToLongTimeString()}', '{endTime.ToLongTimeString()}','{_selfRating}', '{_workoutId}')");
            }
        }



        #region User stuff
        /// <summary>
        /// Registers user and returns user (as well as the UserId).
        /// </summary>
        /// <param name="_user"></param>
        public static User RegisterUser(User _user)
        {
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                string query = $"insert into User(Username, Password, Email, ProPicPath, Birthdate, Weight, Height) values ('{_user.Username.Replace("'", "''")}', '{_user.Password.Replace("'", "''")}', '{_user.Email.Replace("'", "''")}', '{_user.ProPicPath.Replace("'", "''")}', '{_user.Birthdate}', '{_user.Weight}', '{_user.Height}')";
                cnn.Execute(query);
                int id = cnn.Query<int>("SELECT max(Id) FROM User").Single();
                _user.Id = id;
            }
            return _user;
        }
        public static void UpdateUser(User _user)
        {                
            string query = $"Update User SET Username = '{_user.Username.Replace("'", "''")}', Password = '{_user.Password.Replace("'", "''")}', Email = '{_user.Email.Replace("'", "")}', ProPicPath = '{_user.ProPicPath.Replace("'", "''")}', Birthdate = '{_user.Birthdate}', Weight = '{_user.Weight}', Height = '{_user.Height}' WHERE Id = '{_user.Id}'";

            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute(query);
            }
        }

        /// <summary>
        /// returns null if credentials aren't correct
        /// </summary>
        public static User UserLogIn(string _username, string _password)
        {
            User user = null;
            string query = $"SELECT * FROM User WHERE lower(Username) = lower('{_username}') AND Password = '{_password}'";
            using (IDbConnection connection = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                List<User> userList = connection.Query<User>(query).ToList();
                if (userList.Count > 0) user = userList[0];
            }

            return user;
        }

        /// <summary>
        /// returns false is username doesn't exist, true if it does + email in out parameters email
        /// </summary>
        public static bool TryGetUserEmail(string _username, out string email)
        {
            string query = $"SELECT Email FROM User WHERE lower(Username) = lower('{_username}')";
            using (IDbConnection connection = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                var result = connection.Query<string>(query).ToList();
                if (result.Count > 0)
                {
                    email = result.Single();
                }
                else
                {
                    email = "";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get user from Id
        /// </summary>
        internal static User GetUser(int userId)
        {
            User user = null;
            string query = $"SELECT * FROM User WHERE Id = '{userId}'";
            using (IDbConnection connection = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                List<User> userList = connection.Query<User>(query).ToList();
                if (userList.Count > 0) user = userList[0];
            }

            return user;
        }

        /// <summary>
        /// Returns -1 if user doesn't exist
        /// </summary>
        public static int GetUserId(string _username)
        {
            int id = -1;
            string query = $"SELECT Id FROM User WHERE lower(Username) = lower('{_username}')";
            using (IDbConnection connection = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                var result = connection.Query<int>(query).ToList();
                if (result.Count > 0)
                {
                    id = result.Single();
                }
            }

            return id;
        }
        #endregion

        #region ResetTickets stuff
        internal static void SaveResetTicket(string _username, string _tempPwd, DateTime expireDate)
        {
            // get user id
            int userId = GetUserId(_username);
            if (userId == -1) return;

            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute($"insert into ResetTickets(Token, ExpirationDate, TokenUsed, UserId) values ('{_tempPwd}', '{expireDate.ToString()}', '0', '{userId}')");
            }
        }

        /// <summary>
        /// Returns token id if it exists, not expired and not been used already. 
        /// Returns 0 if it exists but has been used.
        /// Returns -1 if it has expired.
        /// Returns -2 if doesn't exist.
        /// </summary>
        internal static int TryGetToken(string _token, int _userId)
        {
            int id = -2;
            string queryExists = $"SELECT Id FROM ResetTickets WHERE Token = '{_token}' AND UserId = {_userId}";
            string queryTokenId = $"SELECT Id FROM ResetTickets WHERE Token = '{_token}' AND UserId = {_userId} AND TokenUsed = 0";
            string queryExpirationDate = $"SELECT ExpirationDate FROM ResetTickets WHERE Id = ";
            using (IDbConnection connection = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                // check if ticket exists with that token and user exists
                var result = connection.Query<int>(queryExists).ToList();
                if (result.Count > 0)
                {
                    id = 0;

                    //check if a ticket with that token, that user and that has not been used exists.
                    var resultToken = connection.Query<int>(queryTokenId).ToList();
                    if (resultToken.Count > 0)
                    {
                        id = -1;

                        int resultTokenId = resultToken.Single();

                        // check if token has expired
                        var resultDate = connection.Query<string>(queryExpirationDate + resultTokenId).ToList();

                        DateTime expirationDate = DateTime.Parse(resultDate.Single());

                        if (DateTime.Now < expirationDate)
                        {
                            // token is not expired!
                            id = resultTokenId;
                        }
                    }
                }
            }
            
            return id;
        }

        internal static void TicketWasUsed(int _ticketId)
        {
            string query = $"UPDATE ResetTickets SET TokenUsed = 1 WHERE Id = {_ticketId}";
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute(query);
            }
        }

        #endregion

        #region Settings

        internal static void SaveSettings(UserSettings _setting)
        {
            string query = $"insert into UserSettings(StartupPageID, IgnoreSelfRating, EmailReport, UserId) values ('{GetIdFromStartupPage(_setting.StartupPage)}', '{_setting.IgnoreSelfRating}', '{_setting.EmailReport}', '{Session.LoggedUser.Id}')";
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute(query);
            }
        }
        internal static void UpdateSettings(UserSettings _setting)
        {
            string query = $"UPDATE UserSettings SET StartupPageID = '{GetIdFromStartupPage(_setting.StartupPage)}', IgnoreSelfRating = '{_setting.IgnoreSelfRating}', EmailReport = '{_setting.EmailReport}' WHERE UserId = '{Session.LoggedUser.Id}'";
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                cnn.Execute(query);
            }
        }
        internal static UserSettings LoadSettings()
        {
            string query = $"SELECT Id, IgnoreSelfRating, EmailReport, StartupPageId FROM UserSettings s WHERE UserId = '{Session.LoggedUser.Id}'";

            UserSettings settings = new UserSettings();
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                var result = cnn.Query<UserSettings>(query);
                if (result.Count() > 0)
                {
                    settings = result.Single();
                }
            }

            return settings;
        }
        internal static string GetStartupPageString(int _id)
        {
            string query = $"SELECT Name FROM StartupPage WHERE Id = {_id}";
            string result;
            using (IDbConnection cnn = new SQLiteConnection(Helper.LoadConnectionString()))
            {
                result = cnn.Query<string>(query).Single();
            }
            return result;
        }
        #endregion

        #region Get ID from...
        private static int GetIdFromItemType(string _muscleGroup)
        {
            switch (_muscleGroup)
            {
                case "Break": return 1;
                case "Distance": return 2;
                case "Reps": return 3;
                case "Duration": return 4;
                case "ToFailure": return 5;
                default: return 1; // whatever
            }
        }
        private static int GetIdFromMuscleGroup(string _muscleGroup)
        {
            switch (_muscleGroup)
            {
                case "Neck": return 1; 
                case "Chest": return 2; 
                case "Biceps": return 4; 
                case "Forearms": return 5; 
                case "Quads": return 7; 
                case "Calves": return 8; 
                case "Triceps": return 10; 
                case "Glutes": return 11; 
                case "Hamstrings": return 12; 
                case "Lower back": return 13; 
                case "Traps": return 14; 
                case "Deltoids": return 15;
                case "Lats": return 16; 
                case "Adductors": return 17; 
                case "Obliques": return 18; 
                case "Abs": return 19;
                default: return 1; // whatever
            }
        }

        private static int GetIdFromExerciseType(string _exerciseType)
        { 
            switch (_exerciseType)
            {
                case "Cardio": return 1;
                case "Dumbbells": return 2;
                case "Barbell": return 3;
                case "Kettlebell": return 4;
                case "Bodyweight": return 5;
                case "Elastic Bands": return 6;
                case "Flexibility": return 7;
                default: return 1; // whatever
            }
        }

        public static int GetIdFromStartupPage(string _startupPage)
        {
            switch (_startupPage)
            {
                case "User Info": return 1;
                case "Workouts": return 2;
                case "Exercises": return 3;
                case "Statistics": return 4;
                case "Settings": return 5;
                default: return 1; // whatever
            }
        }
        #endregion
    }
}
