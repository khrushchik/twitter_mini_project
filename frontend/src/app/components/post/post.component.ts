import { Component, Input, OnDestroy } from '@angular/core';
import { Post } from '../../models/post/post';
import { AuthenticationService } from '../../services/auth.service';
import { AuthDialogService } from '../../services/auth-dialog.service';
import { empty, Observable, Subject } from 'rxjs';
import { DialogType } from '../../models/common/auth-dialog-type';
import { LikeService } from '../../services/like.service';
import { NewComment } from '../../models/comment/new-comment';
import { CommentService } from '../../services/comment.service';
import { User } from '../../models/user';
import { Comment } from '../../models/comment/comment';
import { catchError, switchMap, takeUntil } from 'rxjs/operators';
import { SnackBarService } from '../../services/snack-bar.service';
import { PostService } from '../../services/post.service';
import { GyazoService } from '../../services/gyazo.service';

@Component({
    selector: 'app-post',
    templateUrl: './post.component.html',
    styleUrls: ['./post.component.sass']
})
export class PostComponent implements OnDestroy {
    @Input() public post: Post;
    @Input() public currentUser: User;

    public showComments = false;
    public newComment = {} as NewComment;
    public imageFile: File;
    public showPostContainer = false;
    public showSharePostContainer = false;
    public loading = false;
    public imageUrl: string;
    public email: string;



    private unsubscribe$ = new Subject<void>();

    public constructor(
        private authService: AuthenticationService,
        private authDialogService: AuthDialogService,
        private likeService: LikeService,
        private commentService: CommentService,
        private snackBarService: SnackBarService,
        private postService: PostService,
        private gyazoService: GyazoService
    ) {}

    public ngOnDestroy() {
        this.unsubscribe$.next();
        this.unsubscribe$.complete();
    }

    public toggleComments() {
        if (!this.currentUser) {
            this.catchErrorWrapper(this.authService.getUser())
                .pipe(takeUntil(this.unsubscribe$))
                .subscribe((user) => {
                    if (user) {
                        this.currentUser = user;
                        this.showComments = !this.showComments;
                    }
                });
            return;
        }

        this.showComments = !this.showComments;
    }

    public likePost() {
        if (!this.currentUser) {
            this.catchErrorWrapper(this.authService.getUser())
                .pipe(
                    switchMap((userResp) => this.likeService.likePost(this.post, userResp)),
                    takeUntil(this.unsubscribe$)
                )
                .subscribe((post) => (this.post = post));

            return;
        }

        this.likeService
            .likePost(this.post, this.currentUser)
            .pipe(takeUntil(this.unsubscribe$))
            .subscribe((post) => (this.post = post));
    }

    public dislikePost() {
        return;
    }

    public sharePost() {
        this.postService.sharePost(this.post, this.email).pipe(takeUntil(this.unsubscribe$)).subscribe((post) => (this.post = post));
    }
    public toggleSharePostContainer() {
        this.showSharePostContainer = !this.showSharePostContainer;
    }

    public deletePost() {
        if (this.currentUser.id === this.post.author.id) {
            return this.postService
                .deletePost(this.post)
                .pipe(takeUntil(this.unsubscribe$))
                .subscribe((post) => (this.post = post));
        }
        return alert("You can't delete other people's posts.");
    }

    public updatePost() {
        if (this.currentUser.id === this.post.author.id) {
            const postSubscription = !this.imageFile
                ? this.postService.updatePost(this.post)
                : this.gyazoService.uploadImage(this.imageFile).pipe(
                    switchMap((imageData) => {
                        this.post.previewImage = imageData.url;
                        return this.postService.updatePost(this.post);
                    })
                );
            this.loading = true;

            postSubscription.pipe(takeUntil(this.unsubscribe$)).subscribe(
                (respPost) => {
                    this.post.body = undefined;
                    this.post.previewImage = undefined;
                    this.loading = false;
                },
                (error) => this.snackBarService.showErrorMessage(error)
            );
        }
    }
    public toggleNewPostContainer() {
        this.showPostContainer = !this.showPostContainer;
    }
    public removeImage() {
        this.imageUrl = undefined;
        this.imageFile = undefined;
    }
    public loadImage(target: any) {
        this.imageFile = target.files[0];

        if (!this.imageFile) {
            target.value = '';
            return;
        }

        if (this.imageFile.size / 1000000 > 5) {
            target.value = '';
            this.snackBarService.showErrorMessage(`Image can't be heavier than ~5MB`);
            return;
        }

        const reader = new FileReader();
        reader.addEventListener('load', () => (this.imageUrl = reader.result as string));
        reader.readAsDataURL(this.imageFile);
    }


    public sendComment() {
        this.newComment.authorId = this.currentUser.id;
        this.newComment.postId = this.post.id;

        this.commentService
            .createComment(this.newComment)
            .pipe(takeUntil(this.unsubscribe$))
            .subscribe(
                (resp) => {
                    if (resp) {
                        this.post.comments = this.sortCommentArray(this.post.comments.concat(resp.body));
                        this.newComment.body = undefined;
                    }
                },
                (error) => this.snackBarService.showErrorMessage(error)
            );
    }

    public openAuthDialog() {
        this.authDialogService.openAuthDialog(DialogType.SignIn);
    }

    private catchErrorWrapper(obs: Observable<User>) {
        return obs.pipe(
            catchError(() => {
                this.openAuthDialog();
                return empty();
            })
        );
    }

    private sortCommentArray(array: Comment[]): Comment[] {
        return array.sort((a, b) => +new Date(b.createdAt) - +new Date(a.createdAt));
    }
}
