# Contributing

## Process

Raising and assigning issues, branching, testing, pull requests, code review, and
merging all follow the ABC-practica organisation-wide contribution guide:

**https://github.com/ABC-practica/ABC-practica/blob/main/CONTRIBUTING.md**

Read that first. Everything below is specific to *this* repository and is not
covered by the org guide.

## Repository setup

This is a Unity project. Open the repository root as the project folder — do not
open a subdirectory.

The Unity editor version is pinned in
[`ProjectSettings/ProjectVersion.txt`](ProjectSettings/ProjectVersion.txt).
Install that exact version via Unity Hub before opening the project; opening it
with a different version will silently upgrade and reserialize assets, which
produces very large, hard-to-review diffs.

Do not commit `Library/`, `Temp/`, `obj/`, `Build/`, `Builds/`, `Logs/`, or
`UserSettings/`. They are Unity-local caches, they are already covered by
[`.gitignore`](.gitignore), and committing them is what bloated this project's
history to over a gigabyte before the repository was migrated here.

## Git LFS

Binary assets in this repository are stored in [Git LFS](https://git-lfs.com/).
**Install LFS before your first clone.** If you clone without it, your working
copy will contain small text pointer files where textures, audio, and models
should be, and Unity will fail to import them.

```bash
git lfs install
```

If you already cloned without LFS, you do not need to re-clone — fetch the real
content in place:

```bash
git lfs pull
```

### Two warnings you can expect when cloning

Neither of these means the clone is broken. Check `git status` — if it reports a
clean tree, you have everything.

**`fatal: active post-checkout hook found during git clone`**, followed by
`warning: Clone succeeded, but checkout failed.` This is Git 2.45's clone
protection refusing to run the hook that Git LFS installs. It affects any
LFS-enabled repository and is not specific to this project. Updating Git LFS to
the current release is the real fix; to get past it once, clone with:

```bash
GIT_CLONE_PROTECTION_ACTIVE=false git clone https://github.com/ABC-practica/dacic-zero-game.git
```

**`Filename too long`** on the Animancer sample files. The longest path in this
repository is 133 characters, and Windows applies a 260-character limit to the
full absolute path — so a clone nested more than ~120 characters deep will fail
to check those files out. Clone somewhere short (`C:\dev\...`, not a path buried
under `AppData\Local\Temp`), or lift the limit:

```bash
git config --global core.longpaths true
```

### Which files go to LFS

Patterns are defined in [`.gitattributes`](.gitattributes) and cover textures and
images, audio, video, 3D models, fonts, native libraries, archives, and PDFs.
Matching files are converted to LFS automatically on commit; you do not need to
do anything per-file.

If you add a new kind of large binary asset whose extension is not already
listed, add the pattern to `.gitattributes` in the same pull request that adds
the asset. A pattern only applies to files committed *after* it is added — it
does not retroactively convert anything already in history.

Unity `.meta` files, scenes (`.unity`), prefabs, animations, and `.asset` files
are deliberately **not** in LFS. They are text and need to stay diffable and
mergeable.

### Storage and bandwidth limits

Git LFS on GitHub has a shared free tier per account/organisation: **1 GB of
storage and 1 GB of bandwidth per month**. Bandwidth counts every download of
LFS content, so a full fresh clone by each team member consumes it quickly.

Practical consequences:

- Prefer `git lfs pull` on an existing clone over re-cloning from scratch.
- Avoid committing large binaries you are only trying out. Every version of an
  LFS file is retained and counts against storage, and deleting the file in a
  later commit does not reclaim it.
- Keep imported third-party asset packs to what the project actually uses.

If the organisation hits the quota, LFS becomes read-only — pushes of new binary
assets fail — until a data pack is purchased or the quota resets. Raise it with
the repository admins rather than working around it.
