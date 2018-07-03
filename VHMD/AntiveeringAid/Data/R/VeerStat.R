# Veer data
# VI: Sessions (1 Baseline, 2 Audio, 3 AfterAudio, 4 Vibration, 5 AfterVibration), Sight (0 Blind, 1 Sighted) 
# VD: Error (deviation), Time (seconds)

#dataveer <- read.csv("C:\\Users\\victo\\Desktop\\VeerCSV.csv",header=TRUE)
dataveer <- read.csv("E:\\Dropbox\\Working\\Veer\\VeerClean.csv",header=TRUE)

attach(dataveer)
Sight <- factor(Sight)
Session <- factor(Session)

# Two-way ANOVA and MANOVA
AOVtwoErr <- aov(Error ~ Session * Sight)
summary(AOVtwoErr)
#AOVtwoTime <- aov(Time ~ Session * Sight)
#summary(AOVtwoTime)
#MAOV <- manova(cbind(Error,Time) ~ Session * Sight)
#summary(MAOV)

# Post-hoc
TukeyHSD(AOVtwoErr)
#TukeyHSD(AOVtwoTime)

pairwise.t.test(Error, Session, p.adj = "bonf")
pairwise.t.test(datablind$Error, datablind$Session, p.adj = "bonf")
pairwise.t.test(datasight$Error, datasight$Session, p.adj = "bonf")

#pairwise.t.test(Time, Session, p.adj = "bonf")
#pairwise.t.test(datablind$Time, datablind$Session, p.adj = "bonf")
#pairwise.t.test(datasight$Time, datasight$Session, p.adj = "bonf")

# Charts
boxplot(Error~Sight)
boxplot(Time~Sight)
boxplot(Error~Session)
boxplot(Time~Session)

datablind <- dataveer[dataveer[,"Sight"] == 0,]
datasight <- dataveer[dataveer[,"Sight"] == 1,]

boxplot(datasight$Error ~ datasight$Session)
boxplot(datablind$Error ~ datablind$Session)

# Exit
detach(dataveer)
