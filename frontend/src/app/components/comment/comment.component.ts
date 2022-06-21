import { Component, Input, OnDestroy } from '@angular/core';
import { Comment } from '../../models/comment/comment';
import { catchError, switchMap, takeUntil } from 'rxjs/operators';
import { User } from '../../models/user';
import { empty, Observable, Subject } from 'rxjs';
import { AuthenticationService } from '../../services/auth.service';
import { AuthDialogService } from '../../services/auth-dialog.service';
import { LikeService } from '../../services/like.service';
import { CommentService } from '../../services/comment.service';
import { Post } from '../../models/post/post';
import { DialogType } from '../../models/common/auth-dialog-type';

@Component({
    selector: 'app-comment',
    templateUrl: './comment.component.html',
    styleUrls: ['./comment.component.sass']
})
export class CommentComponent implements OnDestroy {
    @Input() public comment: Comment;
    @Input() public currentUser: User;
    @Input() public post: Post;


    public constructor(
        private authService: AuthenticationService,
        private authDialogService: AuthDialogService,
        private likeService: LikeService,
        private commentService: CommentService
    ) {}

    private unsubscribe$ = new Subject<void>();

    public ngOnDestroy() {
        this.unsubscribe$.next();
        this.unsubscribe$.complete();
    }

    public likeComment() {
        if (!this.currentUser) {
            this.catchErrorWrapper(this.authService.getUser())
                .pipe(
                    switchMap((userResp) => this.likeService.likeComment(this.comment, userResp)),
                    takeUntil(this.unsubscribe$)
                )
                .subscribe((comment) => (this.comment = comment));

            return;
        }

        this.likeService
            .likeComment(this.comment, this.currentUser)
            .pipe(takeUntil(this.unsubscribe$))
            .subscribe((comment) => (this.comment = comment));
    }

    public deleteComment() {
        if (this.currentUser.id === this.comment.author.id) { // currentUser чомусь undefined, тому не видаляються коментарі
            return this.commentService
                .deleteComment(this.comment)
                .pipe(takeUntil(this.unsubscribe$))
                .subscribe((comment) => (this.comment = comment));
        }
        return alert("You can't delete other people's comments.");
    }

    private catchErrorWrapper(obs: Observable<User>) {
        return obs.pipe(
            catchError(() => {
                this.openAuthDialog();
                return empty();
            })
        );
    }
    public openAuthDialog() {
        this.authDialogService.openAuthDialog(DialogType.SignIn);
    }
}
