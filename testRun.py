#!/usr/bin/env python3
"""
Redate all commits on the current branch, spreading them evenly
between April 10 and April 22 (of the current year).

Usage:
    python redate_commits.py           # dry run (preview only)
    python redate_commits.py --apply   # actually rewrite commits
"""

import subprocess
import sys
from datetime import datetime, timedelta, timezone

# ── Config ────────────────────────────────────────────────────────────────────
YEAR = datetime.now().year
START = datetime(YEAR, 4, 10, 9, 0, 0, tzinfo=timezone.utc)   # Apr 10 09:00
END   = datetime(YEAR, 4, 22, 18, 0, 0, tzinfo=timezone.utc)  # Apr 22 18:00
DRY_RUN = "--apply" not in sys.argv
# ─────────────────────────────────────────────────────────────────────────────


def run(cmd: list[str], capture=True) -> str:
    result = subprocess.run(cmd, capture_output=capture, text=True, check=True)
    return result.stdout.strip() if capture else ""


def get_commits() -> list[tuple[str, str, str]]:
    """Return list of (hash, author_date, subject) for the current branch,
    oldest first."""
    log = run(["git", "log", "--reverse", "--format=%H|||%ad|||%s",
               "--date=iso-strict"])
    commits = []
    for line in log.splitlines():
        if line.strip():
            parts = line.split("|||", 2)
            if len(parts) == 3:
                commits.append((parts[0], parts[1], parts[2]))
    return commits


def spread_dates(n: int) -> list[datetime]:
    """Return n evenly-spaced datetimes between START and END."""
    if n == 1:
        return [START + (END - START) / 2]
    delta = (END - START) / (n - 1)
    return [START + delta * i for i in range(n)]


def fmt(dt: datetime) -> str:
    return dt.strftime("%Y-%m-%dT%H:%M:%S %z")


def redate(commits: list[tuple[str, str, str]], dates: list[datetime]) -> None:
    # Build the env-filter script: match each commit hash and override dates
    cases = []
    for (h, _, _), dt in zip(commits, dates):
        date_str = fmt(dt)
        cases.append(
            f'if [ "$GIT_COMMIT" = "{h}" ]; then\n'
            f'    export GIT_AUTHOR_DATE="{date_str}"\n'
            f'    export GIT_COMMITTER_DATE="{date_str}"\n'
            f'fi'
        )
    env_filter = "\n".join(cases)

    run(
        ["git", "filter-branch", "-f", "--env-filter", env_filter, "HEAD"],
        capture=False,
    )


def main() -> None:
    # Sanity check: inside a git repo?
    try:
        run(["git", "rev-parse", "--git-dir"])
    except subprocess.CalledProcessError:
        print("❌  Not inside a git repository.")
        sys.exit(1)

    commits = get_commits()
    if not commits:
        print("No commits found on current branch.")
        sys.exit(0)

    dates = spread_dates(len(commits))

    print(f"{'DRY RUN — ' if DRY_RUN else ''}Redating {len(commits)} commit(s) "
          f"from {fmt(START)}  →  {fmt(END)}\n")
    print(f"{'HASH':>10}  {'NEW DATE':<30}  SUBJECT")
    print("-" * 72)
    for (h, old, subject), dt in zip(commits, dates):
        print(f"{h[:10]}  {fmt(dt):<30}  {subject[:40]}")

    if DRY_RUN:
        print("\n✅  Dry run complete. Run with --apply to rewrite commits.")
        print("    ⚠️  This rewrites history — force-push will be required.")
        return

    print("\n⚠️  Rewriting git history …")
    redate(commits, dates)
    print("\n✅  Done! Commits have been redated.")
    print("    Force-push to update the remote:")
    branch = run(["git", "rev-parse", "--abbrev-ref", "HEAD"])
    print(f"      git push --force origin {branch}")


if __name__ == "__main__":
    main()