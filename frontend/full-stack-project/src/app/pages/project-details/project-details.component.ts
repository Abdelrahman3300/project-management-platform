import { Component, OnInit, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';


import { Project } from '../../models/project';
import { ProjectService } from '../../services/project.service';

import { ProjectMember } from '../../models/project-member';
import { ProjectMemberService } from '../../services/project-member.service';

import { TaskItem } from '../../models/task-item';
import { TaskItemService } from '../../services/task-item.service';

import { Comment } from '../../models/comment';
import { CommentService } from '../../services/comment.service';

import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-project-details',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe],
  templateUrl: './project-details.component.html',
  styleUrl: './project-details.component.css'
})
export class ProjectDetailsComponent implements OnInit {

  readonly authService = inject(AuthService);
  private readonly projectService = inject(ProjectService);
  private readonly projectMemberService = inject(ProjectMemberService);
  
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  private readonly taskItemService = inject(TaskItemService);

  private readonly commentService = inject(CommentService);


  members: ProjectMember[] = [];

  memberLoading = false;
  memberError = '';

  showAddMemberForm = false;

  newMemberUserId = '';
  newMemberRole = 'Member';

  memberSubmitting = false;

  commentsByTask: Record<string, Comment[]> = {};
  commentTextByTask: Record<string, string> = {};
  commentLoading: Record<string, boolean> = {};
  commentSubmitting: Record<string, boolean> = {};
  commentError: Record<string, string> = {};

  project: Project | null = null;

  tasks: TaskItem[] = [];

  tasksLoading = false;
  tasksError = '';
 
  addingTask = false;
  
  loading = true;
 
  newTaskTitle = '';
  newTaskDescription = '';
  newTaskStatus = 'Todo';
  newTaskPriority = 'Medium';
  newTaskDueDate = '';
  newTaskAssignedToUserId = '';
  

  addTaskError = '';
  error = '';
  membersError = '';

  editingTaskId: string | null = null;

  editTaskTitle = '';
  editTaskDescription = '';
  editTaskStatus = 'Todo';
  editTaskPriority = 'Medium';
  editTaskDueDate = '';
  editTaskAssignedToUserId = '';

  updatingTask = false;
  deletingTaskId: string | null = null;
  taskActionError = '';

  ngOnInit(): void {

    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.error = 'Project ID was not provided.';
      this.loading = false;
      return;
    }

    this.loadProject(id);
    this.loadMembers(id);
    this.loadTasks(id);
    
  }

  private loadProject(id: string): void {

    this.projectService.getById(id).subscribe({

      next: (project) => {
        this.project = project;
        this.loading = false;
      },

      error: (error) => {
        console.error(error);
        this.error = 'Failed to load project.';
        this.loading = false;
      }

    });
  }

  private loadMembers(projectId: string): void {
    this.memberLoading = true;
    this.memberError = '';
  
    this.projectMemberService
      .getMembers(projectId)
      .subscribe({
        next: (members) => {
          this.members = members;
          this.memberLoading = false;
        },
        error: (error) => {
          console.error(error);
  
          this.memberError =
            'Failed to load project members.';
  
          this.memberLoading = false;
        }
      });
  }

  removeMember(userId: string): void {
    if (!this.project) {
      return;
    }
  
    this.projectMemberService
      .removeMember(this.project.id, userId)
      .subscribe({
        next: () => {
          this.members =
            this.members.filter(
              member => member.userId !== userId
            );
        },
  
        error: (error) => {
          console.error(error);
  
          if (error.status === 403) {
            this.memberError =
              'Only the project owner can remove members.';
          } else {
            this.memberError =
              'Failed to remove project member.';
          }
        }
      });
  }

  addMember(): void {
    if (!this.project) {
      return;
    }
  
    const userId = this.newMemberUserId.trim();
  
    if (!userId) {
      return;
    }
  
    this.memberSubmitting = true;
    this.memberError = '';
  
    this.projectMemberService
      .addMember(
        this.project.id,
        {
          userId,
          role: this.newMemberRole
        }
      )
      .subscribe({
        next: (member) => {
          this.members = [
            ...this.members,
            member
          ];
  
          this.newMemberUserId = '';
          this.newMemberRole = 'Member';
          this.showAddMemberForm = false;
          this.memberSubmitting = false;
        },
  
        error: (error) => {
          console.error(error);
  
          if (error.status === 403) {
            this.memberError =
              'Only the project owner can add members.';
          } else if (error.status === 400) {
            this.memberError =
              'Project or user does not exist, or the user is already a member.';
          } else {
            this.memberError =
              'Failed to add project member.';
          }
  
          this.memberSubmitting = false;
        }
      });
  }
  
  private loadTasks(projectId: string): void {
    this.tasksLoading = true;
    this.tasksError = '';
  
    this.taskItemService.getByProjectId(projectId).subscribe({
      next: (tasks) => {
        this.tasks = tasks;

        for (const task of tasks) {
          this.commentsByTask[task.id] = [];
          this.commentTextByTask[task.id] = '';
          this.loadComments(task.id);
        }

        this.tasksLoading = false;
      },
      error: (error) => {
        console.error(error);
        this.tasksError = 'Failed to load project tasks.';
        this.tasksLoading = false;
      }
    });
  }
  
  addTask(): void {
    if (!this.project || !this.newTaskTitle.trim()) {
      return;
    }
  
    this.addingTask = true;
    this.addTaskError = '';
  
    this.taskItemService.create(
      this.project.id,
      {
        title: this.newTaskTitle.trim(),
        description: this.newTaskDescription.trim() || null,
        status: this.newTaskStatus,
        priority: this.newTaskPriority,
        dueDate: this.newTaskDueDate || null,
        assignedToUserId: this.newTaskAssignedToUserId || null
      }
    ).subscribe({
      next: (task) => {
        this.tasks.unshift(task);
  
        this.newTaskTitle = '';
        this.newTaskDescription = '';
        this.newTaskStatus = 'Todo';
        this.newTaskPriority = 'Medium';
        this.newTaskDueDate = '';
        this.newTaskAssignedToUserId = '';
  
        this.addingTask = false;
      },
      error: (error) => {
        console.error('Create task error:', error);
        console.error('Status:', error.status);
        console.error('Error body:', error.error);
      
        this.addTaskError =
          error.error?.title ||
          error.error?.message ||
          error.error ||
          'Failed to create task.';
      
        this.addingTask = false;
      }
    });
  }

  startEditTask(task: TaskItem): void {
    this.editingTaskId = task.id;
  
    this.editTaskTitle = task.title;
    this.editTaskDescription = task.description ?? '';
    this.editTaskStatus = task.status;
    this.editTaskPriority = task.priority;
  
    this.editTaskDueDate = task.dueDate
      ? task.dueDate.substring(0, 10)
      : '';
  
    this.editTaskAssignedToUserId = task.assignedToUserId ?? '';
  
    this.taskActionError = '';
  }

  cancelEditTask(): void {
    this.editingTaskId = null;
    this.taskActionError = '';
  }

  updateTask(): void {
    if (!this.project || !this.editingTaskId) {
      return;
    }
  
    if (!this.editTaskTitle.trim()) {
      return;
    }
  
    this.updatingTask = true;
    this.taskActionError = '';
  
    this.taskItemService.update(
      this.project.id,
      this.editingTaskId,
      {
        title: this.editTaskTitle.trim(),
        description: this.editTaskDescription.trim() || null,
        status: this.editTaskStatus,
        priority: this.editTaskPriority,
        dueDate: this.editTaskDueDate || null,
        assignedToUserId: this.editTaskAssignedToUserId || null
      }
    ).subscribe({
      next: (updatedTask) => {
        this.tasks = this.tasks.map(task =>
          task.id === updatedTask.id
            ? updatedTask
            : task
        );
  
        this.editingTaskId = null;
        this.updatingTask = false;
      },
      error: (error) => {
        console.error(error);
        this.taskActionError = 'Failed to update task.';
        this.updatingTask = false;
      }
    });
  }

  deleteTask(taskId: string): void {
    if (!this.project) {
      return;
    }
  
    const confirmed = confirm(
      'Are you sure you want to delete this task?'
    );
  
    if (!confirmed) {
      return;
    }
  
    this.deletingTaskId = taskId;
    this.taskActionError = '';
  
    this.taskItemService
      .delete(this.project.id, taskId)
      .subscribe({
        next: () => {
          this.tasks = this.tasks.filter(
            task => task.id !== taskId
          );
  
          this.deletingTaskId = null;
        },
        error: (error) => {
          console.error(error);
          this.taskActionError = 'Failed to delete task.';
          this.deletingTaskId = null;
        }
      });
  }

  loadComments(taskId: string): void {
    this.commentLoading[taskId] = true;
    this.commentError[taskId] = '';
  
    this.commentService.getByTaskId(taskId).subscribe({
      next: (comments) => {
        this.commentsByTask[taskId] = comments;
        this.commentLoading[taskId] = false;
      },
      error: () => {
        this.commentError[taskId] =
          'Failed to load comments.';
        this.commentLoading[taskId] = false;
      }
    });
  }

  addComment(taskId: string): void {
    const content =
      (this.commentTextByTask[taskId] ?? '').trim();
  
    if (!content)
      return;
  
    this.commentSubmitting[taskId] = true;
    this.commentError[taskId] = '';
  
    this.commentService.add(taskId, content).subscribe({
      next: (comment) => {
        if (!this.commentsByTask[taskId]) {
          this.commentsByTask[taskId] = [];
        }
  
        this.commentsByTask[taskId].push(comment);
  
        this.commentTextByTask[taskId] = '';
        this.commentSubmitting[taskId] = false;
      },
      error: () => {
        this.commentError[taskId] =
          'Failed to add comment.';
        this.commentSubmitting[taskId] = false;
      }
    });
  }

  deleteComment(
    taskId: string,
    commentId: string
  ): void {
    this.commentService.delete(commentId).subscribe({
      next: () => {
        this.commentsByTask[taskId] =
          (this.commentsByTask[taskId] ?? [])
            .filter(comment => comment.id !== commentId);
      },
      error: () => {
        this.commentError[taskId] =
          'Failed to delete comment.';
      }
    });
  }

  

  goBack(): void {
    this.router.navigate(['/']);
  }
}