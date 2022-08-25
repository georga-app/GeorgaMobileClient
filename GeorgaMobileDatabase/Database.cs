﻿using GeorgaMobileDatabase.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GeorgaMobileDatabase;

public partial class Database
{
    private SQLiteAsyncConnection _database;
    private string _filename;
    private string _password;

    private async Task<bool> Init()
    {
        if (String.IsNullOrEmpty(_password) || String.IsNullOrEmpty(_filename))
            return false;

        if (_database != null)
        {
            return true;
        }

        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _filename);
        var options = new SQLiteConnectionString(path, true, _password, postKeyAction: c =>
        {
            c.Execute("PRAGMA cipher_compatibility = 3");
        });
        _database = new SQLiteAsyncConnection(options);
        var tableCreateResult = await _database.CreateTableAsync<Person>();
        if (tableCreateResult != CreateTableResult.Created && tableCreateResult != CreateTableResult.Migrated)
            return false;  // can this even fail?

        return true;
    }

    public Database()
    {
    }

    public async Task<bool> Login(string dbPath, string password)
    {
        _filename = dbPath;
        _password = password;
        return await Init();
    }

    public void Logout()
    {
        _filename = "";
        _password = "";
        _database = null;
    }
}