using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDoApp.Data;
using ToDoApp.Models;
using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        // Configurar o ambiente e o contexto
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<ToDoContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Usando o contexto do EF Core para gerenciar as tarefas
        using (var context = serviceProvider.GetRequiredService<ToDoContext>())
        {
            // Garantir que o banco de dados esteja criado
            context.Database.EnsureCreated();

            bool running = true;
            while (running)
            {
                Console.WriteLine("\nTo-Do List - Escolha uma opção:");
                Console.WriteLine("1. Adicionar Tarefa");
                Console.WriteLine("2. Listar Tarefas");
                Console.WriteLine("3. Marcar Tarefa como Concluída");
                Console.WriteLine("4. Remover Tarefa");
                Console.WriteLine("5. Sair");
                Console.Write("Opção: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddTask(context);
                        break;
                    case "2":
                        ListTasks(context);
                        break;
                    case "3":
                        MarkTaskAsCompleted(context);
                        break;
                    case "4":
                        RemoveTask(context);
                        break;
                    case "5":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }
            }
        }
    }

    static void AddTask(ToDoContext context)
    {
        Console.Write("Descrição da tarefa: ");
        string description = Console.ReadLine();
        var newTask = new TaskItem { Description = description, IsCompleted = false };
        context.Tasks.Add(newTask);
        context.SaveChanges();
        Console.WriteLine("Tarefa adicionada com sucesso!");
    }

    static void ListTasks(ToDoContext context)
    {
        var tasks = context.Tasks.ToList();
        Console.WriteLine("\nLista de Tarefas:");
        foreach (var task in tasks)
        {
            Console.WriteLine($"- {task.Id}: {task.Description} (Concluída: {task.IsCompleted})");
        }
    }

    static void MarkTaskAsCompleted(ToDoContext context)
    {
        Console.Write("ID da tarefa para marcar como concluída: ");
        if (int.TryParse(Console.ReadLine(), out int taskId))
        {
            var task = context.Tasks.Find(taskId);
            if (task != null)
            {
                task.IsCompleted = true;
                context.SaveChanges();
                Console.WriteLine("Tarefa marcada como concluída.");
            }
            else
            {
                Console.WriteLine("Tarefa não encontrada.");
            }
        }
        else
        {
            Console.WriteLine("ID inválido.");
        }
    }

    static void RemoveTask(ToDoContext context)
    {
        Console.Write("ID da tarefa para remover: ");
        if (int.TryParse(Console.ReadLine(), out int taskId))
        {
            var task = context.Tasks.Find(taskId);
            if (task != null)
            {
                context.Tasks.Remove(task);
                context.SaveChanges();
                Console.WriteLine("Tarefa removida com sucesso.");
            }
            else
            {
                Console.WriteLine("Tarefa não encontrada.");
            }
        }
        else
        {
            Console.WriteLine("ID inválido.");
        }
    }
}