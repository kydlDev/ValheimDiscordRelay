# SonarQube ValheimDiscordRelay - OK (Medium/High impact issues: 0; Sonar server gate: ERROR)

Run 2026-11-16 12:00:25, http://localhost:9000/dashboard?id=ValheimDiscordRelay

| Measure | Value |
|---|---|
| branch_coverage | 3.6 |
| bugs | 0 |
| code_smells | 11 |
| coverage | 4.7 |
| duplicated_lines_density | 0.8 |
| line_coverage | 5.2 |
| ncloc | 18700 |
| reliability_rating | 1.0 |
| security_hotspots | 0 |
| security_rating | 2.0 |
| sqale_rating | 1.0 |
| vulnerabilities | 7 |

## Open issues (18), most severe first

- [MINOR VULNERABILITY] ValheimDiscordRelay-Mod/src/Shared/EnglishNames.cs :45 - Pass a timeout to limit the execution time. (csharpsquid:S6444)
- [MINOR VULNERABILITY] ValheimDiscordRelay-Mod/src/Features/PlayerDeaths/Infrastructure/AttackerInfo.cs :18 - Pass a timeout to limit the execution time. (csharpsquid:S6444)
- [MINOR VULNERABILITY] ValheimDiscordRelay-Mod/src/Features/PlayerDeaths/Infrastructure/DeathQuotes.cs :199 - Pass a timeout to limit the execution time. (csharpsquid:S6444)
- [MINOR VULNERABILITY] ValheimDiscordRelay-Mod/src/Shared/EnglishNames.cs :42 - Pass a timeout to limit the execution time. (csharpsquid:S6444)
- [MINOR VULNERABILITY] ValheimDiscordRelay-Mod/src/Features/PlayerDeaths/Infrastructure/AttackerInfo.cs :16 - Pass a timeout to limit the execution time. (csharpsquid:S6444)
- [MINOR VULNERABILITY] ValheimDiscordRelay-Mod/src/Features/BossDeath/Infrastructure/BossQuotes.cs :82 - Pass a timeout to limit the execution time. (csharpsquid:S6444)
- [MINOR VULNERABILITY] ValheimDiscordRelay-Mod/src/Features/BossDeath/Domain/BossCatalog.cs :37 - Pass a timeout to limit the execution time. (csharpsquid:S6444)
- [INFO CODE_SMELL] ValheimDiscordRelay-Mod/tests/Unit/CaptureSchedulerTests.cs :126 - Call independent Assert statements from inside an Assert.Multiple (external_roslyn:NUnit2045)
- [INFO CODE_SMELL] ValheimDiscordRelay-Mod/tests/Unit/PlaybackReportTests.cs :41 - Call independent Assert statements from inside an Assert.Multiple (external_roslyn:NUnit2045)
- [INFO CODE_SMELL] ValheimDiscordRelay-Mod/tests/Unit/BossCatalogTests.cs :43 - Call independent Assert statements from inside an Assert.Multiple (external_roslyn:NUnit2045)
- [INFO CODE_SMELL] ValheimDiscordRelay-Mod/tests/Unit/PlaybackReportTests.cs :24 - Call independent Assert statements from inside an Assert.Multiple (external_roslyn:NUnit2045)
- [INFO CODE_SMELL] ValheimDiscordRelay-Mod/tests/Unit/PlaybackReportTests.cs :15 - Call independent Assert statements from inside an Assert.Multiple (external_roslyn:NUnit2045)
- [INFO CODE_SMELL] ValheimDiscordRelay-Mod/tests/Unit/ViewerMathTests.cs :176 - Use Assert.That(<collection>, Has.Count.EqualTo(<value>) (external_roslyn:NUnit2046)
- [INFO CODE_SMELL] ValheimDiscordRelay-Mod/tests/Unit/CaptureSchedulerTests.cs :19 - Call independent Assert statements from inside an Assert.Multiple (external_roslyn:NUnit2045)
- [INFO CODE_SMELL] ValheimDiscordRelay-Mod/tests/Unit/CaptureSchedulerTests.cs :155 - Call independent Assert statements from inside an Assert.Multiple (external_roslyn:NUnit2045)
- [INFO CODE_SMELL] ValheimDiscordRelay-Mod/tests/Unit/CaptureSchedulerTests.cs :117 - Call independent Assert statements from inside an Assert.Multiple (external_roslyn:NUnit2045)
- [INFO CODE_SMELL] ValheimDiscordRelay-Mod/tests/Unit/CaptureSchedulerTests.cs :179 - Call independent Assert statements from inside an Assert.Multiple (external_roslyn:NUnit2045)
- [INFO CODE_SMELL] ValheimDiscordRelay-Mod/tests/Unit/BossCatalogTests.cs :55 - Call independent Assert statements from inside an Assert.Multiple (external_roslyn:NUnit2045)
